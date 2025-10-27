namespace Domain.Contracts;

public interface IUseCase<TKey, TEntity>
{
    Task ExecuteAsync(TKey key, TEntity entity, CancellationToken cancellationToken = default);
}
