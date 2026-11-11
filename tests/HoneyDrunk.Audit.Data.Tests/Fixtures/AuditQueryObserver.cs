using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HoneyDrunk.Audit.Data.Tests.Fixtures;

internal sealed class AuditQueryObserver : DbCommandInterceptor, IMaterializationInterceptor
{
    public List<string> Commands { get; } = [];

    public List<object> Parameters { get; } = [];

    public int Materialized { get; private set; }

    public Func<Task>? BeforeRead { get; set; }

    public object InitializedInstance(MaterializationInterceptionData materializationData, object entity)
    {
        Materialized++;
        return entity;
    }

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        Commands.Add(command.CommandText);
        Parameters.AddRange(command.Parameters.Cast<DbParameter>().Select(parameter => parameter.Value!));
        if (BeforeRead is { } beforeRead)
        {
            BeforeRead = null;
            await beforeRead();
        }

        return result;
    }

    public void Reset()
    {
        Commands.Clear();
        Parameters.Clear();
        Materialized = 0;
    }
}
