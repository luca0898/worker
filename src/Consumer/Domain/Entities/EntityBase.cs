namespace Domain.Entities;

public abstract class EntityBase : IEntity
{
    public Guid Id { get; set; }
    public bool Deleted { get; set; }
}
