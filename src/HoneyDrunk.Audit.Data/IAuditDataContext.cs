namespace HoneyDrunk.Audit.Data;

/// <summary>
/// Marker interface for HoneyDrunk.Data unit-of-work composition of Audit persistence.
/// </summary>
/// <remarks>
/// Used only as a generic type argument
/// (<see cref="HoneyDrunk.Data.Abstractions.Transactions.IUnitOfWork{T}"/>) so that
/// Audit's persistence boundary is distinct from any other domain registered against
/// the same DI container. Has no members by design — Sonar S2094 wants empty types
/// to be interfaces, and the marker pattern fits.
/// </remarks>
public interface IAuditDataContext
{
}
