using Instructo.Domain.Common;

namespace Instructo.Domain.Entities;

public abstract class BaseAuditableEntity<T> : BaseEntity<T> where T : struct
{
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}