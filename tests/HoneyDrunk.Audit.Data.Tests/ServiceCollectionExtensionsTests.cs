using Xunit;
using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Audit.Data;
using Microsoft.Extensions.DependencyInjection;

namespace HoneyDrunk.Audit.Data.Tests;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddHoneyDrunkAuditData_RegistersAuditServices()
    {
        var services = new ServiceCollection();

        services.AddHoneyDrunkAuditData();

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IAuditLog) && descriptor.ImplementationType == typeof(DataAuditLog));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IAuditQuery) && descriptor.ImplementationType == typeof(DataAuditQuery));

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuditRetentionPolicy>>();
        Assert.Equal(AuditRetentionPolicy.DefaultRetention, options.Value.Retention);
    }
}
