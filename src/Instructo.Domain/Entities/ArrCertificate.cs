using Domain.Common;
using Domain.Entities.SchoolEntities;
using Domain.Enums;

namespace Domain.Entities;

public class ArrCertificate : BaseEntity<ARRCertificateType>
{
    private ArrCertificate(ARRCertificateType aRRCertificateType, string name, string description)
    {
        Id=aRRCertificateType;
        Name=name;
        Description=description;
    }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public virtual List<School> Schools { get; private set; } = [];
    public static ArrCertificate Create(ARRCertificateType aRRCertificateType, string name, string description)
    {
        return new ArrCertificate(aRRCertificateType, name, description);
    }

    private ArrCertificate() { }
}
