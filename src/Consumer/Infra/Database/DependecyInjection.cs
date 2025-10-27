using Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Database;

public static class DependecyInjection
{
    public static void AddInfraDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>(name: "Database", failureStatus: HealthStatus.Unhealthy, tags: ["ready"]);

        services.AddScoped<IMaterialRepository, MaterialRepository>();
    }
}
