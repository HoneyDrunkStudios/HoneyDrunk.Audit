using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Data.Abstractions.Transactions;
using Microsoft.Extensions.Logging;

namespace HoneyDrunk.Audit.Data;

/// <summary>
/// HoneyDrunk.Data-backed append-only audit writer.
/// </summary>
public sealed partial class DataAuditLog(
    IUnitOfWork<IAuditDataContext> unitOfWork,
    ILogger<DataAuditLog> logger) : IAuditLog
{
    private readonly IUnitOfWork<IAuditDataContext> _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ILogger<DataAuditLog> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task AppendAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var normalized = entry.NormalizeForAppend(DateTimeOffset.UtcNow);
        var record = AuditRecord.FromEntry(normalized);

        await _unitOfWork.Repository<AuditRecord>().AddAsync(record, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        LogAppendedEntry(_logger, record.Id, record.Category, record.Outcome);
    }

    // Source-generated logger method — argument evaluation is gated on
    // IsEnabled(LogLevel.Information) inside the generated code (Sonar
    // CA1848 / S6664 — "evaluation of this argument may be expensive…").
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Appended audit entry {AuditEntryId} category {AuditCategory} outcome {AuditOutcome}")]
    private static partial void LogAppendedEntry(
        ILogger logger,
        string auditEntryId,
        AuditCategory auditCategory,
        AuditOutcome auditOutcome);
}
