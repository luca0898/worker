using Bogus;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Worker.BackgroundServices
{
    public class MaterialsConsumer(
        IServiceProvider serviceProvider,
        IOptionsMonitor<ConsumersConfigurations> consumersConfigurations,
        ILogger<MaterialsConsumer> logger,
        IHostApplicationLifetime applicationLifetime) : BackgroundService
    {
        private readonly Faker<Material> _materialFaker = new Faker<Material>("pt_BR")
            .RuleFor(m => m.Id, _ => Guid.NewGuid())
            .RuleFor(m => m.Deleted, _ => false)
            .RuleFor(m => m.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(m => m.CreatedBy, _ => Guid.NewGuid())
            .RuleFor(m => m.ModifiedAt, _ => null)
            .RuleFor(m => m.ModifiedBy, _ => null)
            .RuleFor(m => m.Name, f => f.Commerce.ProductName())
            .RuleFor(m => m.Description, f => f.Commerce.ProductDescription())
            .RuleFor(m => m.Price, f => Math.Round(f.Random.Decimal(1, 2000), 2))
            .RuleFor(m => m.StockQuantity, f => f.Random.Int(0, 500));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using (var healthScope = serviceProvider.CreateScope())
                {
                    var healthCheckService = healthScope.ServiceProvider.GetRequiredService<HealthCheckService>();
                    var healthReport = await healthCheckService.CheckHealthAsync(healthCheck => healthCheck.Tags.Contains("ready"), stoppingToken);

                    if (healthReport.Status != HealthStatus.Healthy)
                    {
                        logger.LogError("Database health check failed with status {Status}. Stopping application ...", healthReport.Status);
                        applicationLifetime.StopApplication();
                        return;
                    }
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    var material = _materialFaker.Generate();

                    using var scope = serviceProvider.CreateScope();
                    var useCase = scope.ServiceProvider.GetRequiredService<ICreateMaterialUseCase>();

                    await useCase.ExecuteAsync(material.Id, material, stoppingToken);

                    await Task.Delay(consumersConfigurations.CurrentValue.ConsumerDelayMs, stoppingToken);
                }
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
            {
                logger.LogWarning(e, "Consumer has been cancelled");
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred in MaterialsConsumer, stopping application ...");
                applicationLifetime.StopApplication();
            }
        }
    }
}
