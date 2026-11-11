using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Data.Abstractions.Repositories;
using HoneyDrunk.Data.Abstractions.Transactions;
using System.Linq.Expressions;

namespace HoneyDrunk.Audit.Data;

/// <summary>
/// HoneyDrunk.Data-backed forensic audit reader.
/// </summary>
/// <param name="unitOfWork">The scoped unit of work used for audit persistence.</param>
public sealed class DataAuditQuery(IUnitOfWork<IAuditDataContext> unitOfWork) : IBoundedAuditQuery
{
    private readonly IUnitOfWork<IAuditDataContext> _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditEntry>> ReadAsync(
        AuditQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        ValidateWindow(filter);
        if (filter.Limit is > 0)
        {
            return await ReadBoundedAsync(filter, descending: false, cancellationToken).ConfigureAwait(false);
        }

        // Preserve the existing all-results contract only when the caller did not request a positive limit.
        var records = await _unitOfWork.Repository<AuditRecord>().FindAsync(
            BuildFilterExpression(filter),
            cancellationToken).ConfigureAwait(false);

        return
        [
            .. records
                .OrderBy(record => record.OccurredAt)
                .ThenBy(record => record.Id)
                .Select(record => record.ToEntry()),
        ];
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<AuditEntry>> ReadLatestAsync(
        AuditQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        ValidateWindow(filter);
        if (filter.Limit is null or <= 0)
        {
            throw new ArgumentException("A positive limit is required for a bounded audit read.", nameof(filter));
        }

        return ReadBoundedAsync(filter, descending: true, cancellationToken);
    }

    private static void ValidateWindow(AuditQueryFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        if (filter.Until < filter.Since)
        {
            throw new ArgumentException("Until must be greater than or equal to Since.", nameof(filter));
        }
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

    private async Task<IReadOnlyList<AuditEntry>> ReadBoundedAsync(
        AuditQueryFilter filter,
        bool descending,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_unitOfWork.Repository<AuditRecord>() is not IBoundedReadOnlyRepository<AuditRecord> repository)
        {
            throw new NotSupportedException("The audit repository must support source-bounded ordered reads.");
        }

        var records = await repository.FindOrderedAsync(
            BuildFilterExpression(filter),
            record => record.OccurredAt,
            record => record.Id,
            filter.Limit!.Value,
            descending,
            cancellationToken).ConfigureAwait(false);

        return [.. records.Select(record => record.ToEntry())];
    }
}
