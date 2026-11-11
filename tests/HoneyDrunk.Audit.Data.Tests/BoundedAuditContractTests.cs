using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Data.Abstractions.Repositories;
using HoneyDrunk.Data.Abstractions.Transactions;
using NSubstitute;
using Xunit;

namespace HoneyDrunk.Audit.Data.Tests;

public sealed class BoundedAuditContractTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BoundedRead_LegacyRepositoryFailsWithoutReading(bool newestFirst)
    {
        var repository = Substitute.For<IRepository<AuditRecord>>();
        var unitOfWork = Substitute.For<IUnitOfWork<IAuditDataContext>>();
        unitOfWork.Repository<AuditRecord>().Returns(repository);
        var query = new DataAuditQuery(unitOfWork);
        var filter = new AuditQueryFilter(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, Limit: 50);

        await Assert.ThrowsAsync<NotSupportedException>(() => newestFirst
            ? query.ReadLatestAsync(filter) : query.ReadAsync(filter));

        Assert.Empty(repository.ReceivedCalls());
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ReadLatestAsync_RequiresPositiveLimit(int? limit)
    {
        var unitOfWork = Substitute.For<IUnitOfWork<IAuditDataContext>>();
        var query = new DataAuditQuery(unitOfWork);

        await Assert.ThrowsAsync<ArgumentException>(() => query.ReadLatestAsync(
            new AuditQueryFilter(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, Limit: limit)));

        Assert.Empty(unitOfWork.ReceivedCalls());
    }

    [Fact]
    public async Task ReadLatestAsync_ValidatesFilterBeforeRepositoryAccess()
    {
        var unitOfWork = Substitute.For<IUnitOfWork<IAuditDataContext>>();
        var query = new DataAuditQuery(unitOfWork);
        await Assert.ThrowsAsync<ArgumentNullException>(() => query.ReadLatestAsync(null!));
        await Assert.ThrowsAsync<ArgumentException>(() => query.ReadLatestAsync(
            new AuditQueryFilter(DateTimeOffset.MaxValue, DateTimeOffset.MinValue, Limit: 1)));
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => query.ReadLatestAsync(
            new AuditQueryFilter(DateTimeOffset.MinValue, DateTimeOffset.MaxValue, Limit: 1), cancellation.Token));
        Assert.Empty(unitOfWork.ReceivedCalls());
    }

    [Fact]
    public void BoundedCapability_DoesNotChangeExistingConstructorDeconstructionOrQueryInterface()
    {
        Assert.Single(typeof(IAuditQuery).GetMethods());
        Assert.Equal("ReadAsync", typeof(IAuditQuery).GetMethods()[0].Name);
        Assert.Single(typeof(AuditQueryFilter).GetConstructors());
        Assert.Equal(10, typeof(AuditQueryFilter).GetConstructors()[0].GetParameters().Length);
        Assert.Equal(10, typeof(AuditQueryFilter).GetMethod("Deconstruct")!.GetParameters().Length);
        Assert.DoesNotContain(typeof(IBoundedAuditQuery), typeof(IAuditQuery).GetInterfaces());
    }
}
