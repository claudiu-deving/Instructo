using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Common;

using NetTopologySuite.Geometries;

namespace Domain.Entities;

public class City : IEntity
{

    public int Id { get; set; }
    public required string Name { get; set; }
    public int CountyId { get; set; }
    public virtual County? County { get; set; }
    public Point? Location { get; set; }
}
