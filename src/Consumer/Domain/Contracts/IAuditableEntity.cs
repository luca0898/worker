namespace Domain.Contracts;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; init; }
    Guid CreatedBy { get; init; }
    DateTime? ModifiedAt { get; init; }
    Guid? ModifiedBy { get; init; }
}
