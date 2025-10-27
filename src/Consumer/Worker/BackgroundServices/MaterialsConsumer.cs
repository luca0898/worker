using Bogus;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Worker.BackgroundServices
{
    public class MaterialsConsumer(
        IServiceProvider serviceProvider,
        IOptionsMonitor<ConsumersConfigurations> consumersConfigurations,
        ILogger<MaterialsConsumer> logger,
        IHostApplicationLifetime applicationLifetime) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var faker = new Faker<Material>("pt_BR")
                        .RuleFor(m => m.Id, f => Guid.NewGuid())
                        .RuleFor(m => m.Deleted, f => false)
                        .RuleFor(m => m.CreatedAt, f => DateTime.UtcNow)
                        .RuleFor(m => m.CreatedBy, f => Guid.NewGuid())
                        .RuleFor(m => m.ModifiedAt, f => null)
                        .RuleFor(m => m.ModifiedBy, f => null)
                        .RuleFor(m => m.Name, f => f.Commerce.ProductName())
                        .RuleFor(m => m.Description, f => f.Commerce.ProductDescription())
                        .RuleFor(m => m.Price, f => Math.Round(f.Random.Decimal(1, 2000), 2))
                        .RuleFor(m => m.StockQuantity, f => f.Random.Int(0, 500));

                    var material = faker.Generate();

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
