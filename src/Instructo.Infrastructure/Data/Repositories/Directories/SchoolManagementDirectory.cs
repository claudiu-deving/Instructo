using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Infrastructure.Data.Repositories.Directories;
public class SchoolManagementDirectory(
    AppDbContext appDbContext,
    ISchoolQueriesRepository SchoolQueriesRepository,
    ISchoolCommandRepository SchoolCommandRepository,
    IQueryRepository<City, int> CityQueriesRepository,
    IQueryRepository<ArrCertificate, ARRCertificateType> CertificateQueriesRepository,
    IQueryRepository<VehicleCategory, VehicleCategoryType> VehicleQueriesRepository) : UnitOfWork(appDbContext), ISchoolManagementDirectory
{
    public ISchoolQueriesRepository SchoolQueriesRepository { get; } = SchoolQueriesRepository;

    public ISchoolCommandRepository SchoolCommandRepository { get; } = SchoolCommandRepository;

    public IQueryRepository<City, int> CityQueriesRepository { get; } = CityQueriesRepository;

    public IQueryRepository<ArrCertificate, ARRCertificateType> CertificateQueriesRepository { get; } = CertificateQueriesRepository;

    public IQueryRepository<VehicleCategory, VehicleCategoryType> VehicleQueriesRepository { get; } = VehicleQueriesRepository;
}
