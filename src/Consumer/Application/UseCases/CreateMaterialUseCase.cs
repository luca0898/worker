using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class CreateMaterialUseCase(IMaterialRepository repository, ILogger<CreateMaterialUseCase> logger) : ICreateMaterialUseCase
{
    public async Task ExecuteAsync(Guid key, Material entity, CancellationToken cancellationToken = default)
    {
        if (key == Guid.Empty)
            throw new ArgumentException("The provided key is invalid.", nameof(key));

        logger.LogInformation("Creating material with key: {Key}", key);

        await repository.CreateAsync(entity, cancellationToken);
    }
}
