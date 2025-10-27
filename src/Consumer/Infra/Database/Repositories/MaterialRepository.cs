using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class MaterialRepository(ApplicationDbContext context) : IMaterialRepository
{
    private readonly DbSet<Material> _materials = context.Set<Material>();

    public async Task CreateAsync(Material material, CancellationToken cancellationToken = default)
    {
        await _materials.AddAsync(material, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => _materials.AnyAsync(material => material.Id == id, cancellationToken);
}
