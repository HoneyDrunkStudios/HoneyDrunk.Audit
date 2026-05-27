using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Data.Abstractions.Transactions;
using System.Linq.Expressions;

namespace HoneyDrunk.Audit.Data;

/// <summary>
/// HoneyDrunk.Data-backed forensic audit reader.
/// </summary>
public sealed class DataAuditQuery(IUnitOfWork<AuditDataContext> unitOfWork) : IAuditQuery
{
    private readonly IUnitOfWork<AuditDataContext> _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditEntry>> ReadAsync(
        AuditQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        if (filter.Until < filter.Since)
        {
            throw new ArgumentException("Until must be greater than or equal to Since.", nameof(filter));
        }

        var records = await _unitOfWork.Repository<AuditRecord>().FindAsync(
            BuildFilterExpression(filter),
            cancellationToken).ConfigureAwait(false);

        var ordered = records
            .OrderBy(record => record.OccurredAt)
            .ThenBy(record => record.Id)
            .Select(record => record.ToEntry());

        if (filter.Limit is > 0)
        {
            ordered = ordered.Take(filter.Limit.Value);
        }

        return [.. ordered];
    }

    private static Expression<Func<AuditRecord, bool>> BuildFilterExpression(AuditQueryFilter filter)
    {
        var record = Expression.Parameter(typeof(AuditRecord), "record");
        Expression body = Expression.Constant(true);

        body = And(body, GreaterThanOrEqual(record, nameof(AuditRecord.OccurredAt), filter.Since));
        body = And(body, LessThanOrEqual(record, nameof(AuditRecord.OccurredAt), filter.Until));
        body = AndIfValue(body, record, nameof(AuditRecord.Actor), filter.Actor);
        body = AndIfValue(body, record, nameof(AuditRecord.Category), filter.Category);
        body = AndIfValue(body, record, nameof(AuditRecord.EventName), filter.EventName);
        body = AndIfValue(body, record, nameof(AuditRecord.TargetType), filter.TargetType);
        body = AndIfValue(body, record, nameof(AuditRecord.TargetId), filter.TargetId);
        body = AndIfValue(body, record, nameof(AuditRecord.CorrelationId), filter.CorrelationId);
        body = AndIfValue(body, record, nameof(AuditRecord.TenantId), filter.TenantId?.ToString());

        return Expression.Lambda<Func<AuditRecord, bool>>(body, record);
    }

    private static BinaryExpression And(Expression left, Expression right) => Expression.AndAlso(left, right);

    private static BinaryExpression GreaterThanOrEqual<TValue>(
        Expression instance,
        string propertyName,
        TValue value)
    {
        return Expression.GreaterThanOrEqual(
            Expression.Property(instance, propertyName),
            Expression.Constant(value, typeof(TValue)));
    }

    private static BinaryExpression LessThanOrEqual<TValue>(
        Expression instance,
        string propertyName,
        TValue value)
    {
        return Expression.LessThanOrEqual(
            Expression.Property(instance, propertyName),
            Expression.Constant(value, typeof(TValue)));
    }

    private static Expression AndIfValue<TValue>(
        Expression current,
        Expression instance,
        string propertyName,
        TValue? value)
    {
        if (value is null)
        {
            return current;
        }

        var property = Expression.Property(instance, propertyName);
        var constant = Expression.Constant(value, property.Type);
        return And(current, Expression.Equal(property, constant));
    }
}
