using HoneyDrunk.Audit.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace HoneyDrunk.Audit.Data;

/// <summary>
/// Dependency injection extensions for HoneyDrunk.Audit.Data.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Data-backed Audit runtime services. The host must also compose
    /// <c>IUnitOfWork&lt;IAuditDataContext&gt;</c> through HoneyDrunk.Data.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddHoneyDrunkAuditData(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<AuditRetentionPolicy>();
        services.AddScoped<IAuditLog, DataAuditLog>();
        services.AddScoped<IAuditQuery, DataAuditQuery>();

        return services;
    }
}
