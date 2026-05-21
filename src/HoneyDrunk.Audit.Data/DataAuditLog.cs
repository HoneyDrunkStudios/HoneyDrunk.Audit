using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Data.Abstractions.Transactions;
using Microsoft.Extensions.Logging;

namespace HoneyDrunk.Audit.Data;

/// <summary>
/// HoneyDrunk.Data-backed append-only audit writer.
/// </summary>
public sealed class DataAuditLog(
    IUnitOfWork<AuditDataContext> unitOfWork,
    ILogger<DataAuditLog> logger) : IAuditLog
{
    private readonly IUnitOfWork<AuditDataContext> _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ILogger<DataAuditLog> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task AppendAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var normalized = entry.NormalizeForAppend(DateTimeOffset.UtcNow);
        var record = AuditRecord.FromEntry(normalized);

        await _unitOfWork.Repository<AuditRecord>().AddAsync(record, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation(
            "Appended audit entry {AuditEntryId} category {AuditCategory} outcome {AuditOutcome}",
            record.Id,
            record.Category,
            record.Outcome);
    }
}
