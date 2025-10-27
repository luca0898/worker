namespace Domain.Entities;

public abstract class AuditableEntity : EntityBase, IAuditableEntity
{
    public required DateTime CreatedAt { get; init; }
    public required Guid CreatedBy { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public Guid? ModifiedBy { get; init; }
}
