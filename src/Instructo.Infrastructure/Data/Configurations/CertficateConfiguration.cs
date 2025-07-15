using Domain.Entities;
using Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Configurations;
internal static class CertficateConfiguration
{
    public static void ConfigureArrCertificates(this ModelBuilder builder)
    {
        builder.Entity<ArrCertificate>().ToTable("ARRCertificates");
        builder.Entity<ArrCertificate>().HasKey(e => e.Id);
        builder.Entity<ArrCertificate>().Property(e => e.Id);
        builder.Entity<ArrCertificate>().Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Entity<ArrCertificate>().Property(e => e.Description)
            .HasMaxLength(500);
        SeedCertificateTypes(builder);
    }

    private static void SeedCertificateTypes(ModelBuilder modelBuilder)
    {
        var certificateTypes = new List<ArrCertificate>
        {
            ArrCertificate.Create(
                ARRCertificateType.FreightTransport,
                "Atestat pentru transport marfă",
                "Certificate for general goods transportation"
            ),
            ArrCertificate.Create(
                ARRCertificateType.PassengerTransport,
                "Atestat pentru transport persoane",
                "Certificate for passenger transportation"
            ),
            ArrCertificate.Create(
                ARRCertificateType.DangerousGoodsTransport,
                "Atestat ADR",
                "Certificate for dangerous goods transportation (ADR)"
            ),
            ArrCertificate.Create(
                ARRCertificateType.OversizedTransport,
                "Atestat pentru transport agabaritic",
                "Certificate for oversized load transportation"
            ),
            ArrCertificate.Create(
                ARRCertificateType.TaxiTransport,
                "Atestat pentru transport taxi",
                "Certificate for taxi transportation"
            ),
            ArrCertificate.Create(
                ARRCertificateType.TransportManager,
                "Atestat pentru manager de transport (CPI)",
                "Certificate for transport managers (CPI)"
            ),
            ArrCertificate.Create(
                ARRCertificateType.DrivingInstructor,
                "Atestat pentru instructor auto",
                "Certificate for driving instructors"
            ),
            ArrCertificate.Create(
                ARRCertificateType.RoadLegislationTeacher,
                "Atestat pentru profesor de legislație rutieră",
                "Certificate for road legislation teachers"
            ),
            ArrCertificate.Create(
                ARRCertificateType.DangerousGoodsSafetyAdvisor,
                "Atestat pentru consilier de siguranță",
                "Certificate for safety advisors for the transport of dangerous goods"
            )
        };

        modelBuilder.Entity<ArrCertificate>().HasData(certificateTypes);
    }
}
