using Xunit;
using System.Linq.Expressions;
using HoneyDrunk.Data.Abstractions.Repositories;
using HoneyDrunk.Data.Abstractions.Transactions;

namespace HoneyDrunk.Audit.Data.Tests.Fixtures;

internal sealed class FakeAuditUnitOfWork : IUnitOfWork<HoneyDrunk.Audit.Data.AuditDataContext>
{
    private readonly FakeAuditRecordRepository _repository = new();

    public bool HasPendingChanges { get; private set; }

    public int SaveChangesCalls { get; private set; }

    public IReadOnlyList<HoneyDrunk.Audit.Data.AuditRecord> Records => _repository.Records;

    public IRepository<TEntity> Repository<TEntity>()
        where TEntity : class
    {
        if (typeof(TEntity) == typeof(HoneyDrunk.Audit.Data.AuditRecord))
        {
            return (IRepository<TEntity>)(object)_repository;
        }

        throw new NotSupportedException(typeof(TEntity).FullName);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        HasPendingChanges = false;
        SaveChangesCalls++;
        return Task.FromResult(1);
    }

    public Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<ITransactionScope>(new FakeTransactionScope());
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private sealed class FakeAuditRecordRepository : IRepository<HoneyDrunk.Audit.Data.AuditRecord>
    {
        private readonly List<HoneyDrunk.Audit.Data.AuditRecord> _records = [];

        public IReadOnlyList<HoneyDrunk.Audit.Data.AuditRecord> Records => _records;

        public Task AddAsync(HoneyDrunk.Audit.Data.AuditRecord entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _records.Add(entity);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<HoneyDrunk.Audit.Data.AuditRecord> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _records.AddRange(entities);
            return Task.CompletedTask;
        }

        public ValueTask<HoneyDrunk.Audit.Data.AuditRecord?> FindByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(_records.FirstOrDefault(record => record.Id == id.ToString()));
        }

        public Task<IReadOnlyList<HoneyDrunk.Audit.Data.AuditRecord>> FindAsync(
            Expression<Func<HoneyDrunk.Audit.Data.AuditRecord, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult<IReadOnlyList<HoneyDrunk.Audit.Data.AuditRecord>>(
                _records.Where(predicate.Compile()).ToArray());
        }

        public Task<HoneyDrunk.Audit.Data.AuditRecord?> FindOneAsync(
            Expression<Func<HoneyDrunk.Audit.Data.AuditRecord, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_records.FirstOrDefault(predicate.Compile()));
        }

        public Task<bool> ExistsAsync(
            Expression<Func<HoneyDrunk.Audit.Data.AuditRecord, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_records.Any(predicate.Compile()));
        }

        public Task<int> CountAsync(
            Expression<Func<HoneyDrunk.Audit.Data.AuditRecord, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(predicate is null ? _records.Count : _records.Count(predicate.Compile()));
        }

        public void Update(HoneyDrunk.Audit.Data.AuditRecord entity) => throw new NotSupportedException();

        public void UpdateRange(IEnumerable<HoneyDrunk.Audit.Data.AuditRecord> entities) => throw new NotSupportedException();

        public void Remove(HoneyDrunk.Audit.Data.AuditRecord entity) => throw new NotSupportedException();

        public void RemoveRange(IEnumerable<HoneyDrunk.Audit.Data.AuditRecord> entities) => throw new NotSupportedException();
    }

    private sealed class FakeTransactionScope : ITransactionScope
    {
        public Guid TransactionId { get; } = Guid.NewGuid();

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
