using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Infrastructure.Data.Repositories.Directories;
public class SchoolManagementDirectory(
    AppDbContext appDbContext,
    ISchoolQueriesRepository SchoolQueriesRepository,
    ISchoolCommandRepository SchoolCommandRepository,
    IQueryRepository<City, int> CityQueriesRepository,
    IQueryRepository<ArrCertificate, int> CertificateQueriesRepository,
    IQueryRepository<VehicleCategory, int> VehicleQueriesRepository) : UnitOfWork(appDbContext), ISchoolManagementDirectory
{
    public ISchoolQueriesRepository SchoolQueriesRepository { get; } = SchoolQueriesRepository;

    public ISchoolCommandRepository SchoolCommandRepository { get; } = SchoolCommandRepository;

    public IQueryRepository<City, int> CityQueriesRepository { get; } = CityQueriesRepository;

    public IQueryRepository<ArrCertificate, int> CertificateQueriesRepository { get; } = CertificateQueriesRepository;

    public IQueryRepository<VehicleCategory, int> VehicleQueriesRepository { get; } = VehicleQueriesRepository;
}
