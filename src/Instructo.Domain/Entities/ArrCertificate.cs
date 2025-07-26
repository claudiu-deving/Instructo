using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class ArrCertificate : IEntity
{
    private ArrCertificate(int id, string name, string description)
    {
        Id=id;
        Name=name;
        Description=description;
    }

    private ArrCertificate()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public virtual List<School> Schools { get; private set; } = [];
    public int Id { get; }

    public static ArrCertificate Create(ARRCertificateType aRRCertificateType, string name, string description)
    {
        return new ArrCertificate((int)aRRCertificateType, name, description);
    }
}