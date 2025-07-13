using Domain.Common;
using Domain.Entities.SchoolEntities;

namespace Domain.Entities;

public class SchoolCertificate : IEntity
{
    public Guid SchoolId { get; set; }
    public int CertificateId { get; set; }
    public School School { get; set; } = null!;
    public ArrCertificate Certificate { get; set; } = null!;
}