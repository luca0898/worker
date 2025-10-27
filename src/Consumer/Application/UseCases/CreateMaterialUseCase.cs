using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class CreateMaterialUseCase(IMaterialRepository repository, ILogger<CreateMaterialUseCase> logger) : ICreateMaterialUseCase
{
    public async Task ExecuteAsync(Guid key, Material entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (key == Guid.Empty)
            throw new ArgumentException("The provided key is invalid.", nameof(key));

        if (entity.Id != key)
            throw new ArgumentException("The material identifier must match the provided key.", nameof(entity));

        if (string.IsNullOrWhiteSpace(entity.Name))
            throw new ArgumentException("Material name is required.", nameof(entity));

        if (entity.Price <= 0)
            throw new ArgumentOutOfRangeException(nameof(entity), "Material price must be greater than zero.");

        if (entity.StockQuantity < 0)
            throw new ArgumentOutOfRangeException(nameof(entity), "Material stock quantity cannot be negative.");

        logger.LogInformation("Creating material with key: {Key}", key);

        if (await repository.ExistsAsync(key, cancellationToken))
        {
            logger.LogWarning("Material with key {Key} already exists. Skipping creation.", key);
            return;
        }

        await repository.CreateAsync(entity, cancellationToken);
    }
}
