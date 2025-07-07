using NetTopologySuite.Geometries;

namespace Domain.Entities;

public class County
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public ICollection<City>? Cities { get; set; }
}