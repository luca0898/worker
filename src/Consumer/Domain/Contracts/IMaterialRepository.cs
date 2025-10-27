namespace Domain.Contracts;

public interface IMaterialRepository
{
    Task CreateAsync(Material material, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
