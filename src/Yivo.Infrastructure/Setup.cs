using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Yivo.Infrastructure;

public static class Setup
{
    public static IServiceCollection AddYivoSqliteContext(this IServiceCollection services, string conn)
    {
        if (string.IsNullOrEmpty(conn))
            conn = $"Data Source={Path.Combine(AppContext.BaseDirectory, "yivo.db")}";

        services.AddDbContext<YivoContext>(options =>
        {
            options.UseSqlite(conn,
                x => x.MigrationsAssembly(typeof(YivoContext).Assembly.GetName().Name));
        });

        return services;
    }
}