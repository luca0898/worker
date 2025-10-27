namespace Domain.Contracts;

public interface IMaterialRepository
{
    Task CreateAsync(Material material, CancellationToken cancellationToken = default);
}
