namespace Domain.Contracts;

public interface IEntity
{
    Guid Id { get; set; }
    bool Deleted { get; set; }
}
