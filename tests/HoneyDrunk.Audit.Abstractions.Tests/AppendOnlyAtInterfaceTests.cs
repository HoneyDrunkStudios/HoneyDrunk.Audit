using Xunit;
using HoneyDrunk.Audit.Abstractions;

namespace HoneyDrunk.Audit.Abstractions.Tests;

public sealed class AppendOnlyAtInterfaceTests
{
    [Fact]
    public void IAuditLog_OnlyExposesAppendAsync()
    {
        var methods = typeof(IAuditLog).GetMethods().Select(method => method.Name).ToArray();

        Assert.Equal([nameof(IAuditLog.AppendAsync)], methods);
        Assert.DoesNotContain(methods, name => name.Contains("Update", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(methods, name => name.Contains("Delete", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(methods, name => name.Contains("Remove", StringComparison.OrdinalIgnoreCase));
    }
}
