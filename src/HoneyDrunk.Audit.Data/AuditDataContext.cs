namespace HoneyDrunk.Audit.Data;

/// <summary>
/// Marker type for HoneyDrunk.Data unit-of-work composition of Audit persistence.
/// </summary>
/// <remarks>
/// Abstract because the type is only ever used as a generic type argument
/// (<see cref="HoneyDrunk.Data.Abstractions.Transactions.IUnitOfWork{T}"/>) — there's
/// no need (or correct path) to instantiate it. The private-ctor sealed-class form
/// tripped Sonar S3453 "this class can't be instantiated"; abstract gives the same
/// "don't instantiate" signal without the suspicious shape.
/// </remarks>
public abstract class AuditDataContext
{
}
