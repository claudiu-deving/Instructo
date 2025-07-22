using Domain.Common;

namespace Domain.Entities;
public class Transmission : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
