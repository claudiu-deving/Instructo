using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

using Infrastructure.Data.Repositories.Queries;

namespace Infrastructure.Data.Repositories.Directories;
public class SchoolManagementDirectory(
    AppDbContext appDbContext,
    ISchoolQueriesRepository schoolQueriesRepository,
    ISchoolCommandRepository schoolCommandRepository,
    IQueryRepository<City, int> cityQueriesRepository,
    IQueryRepository<ArrCertificate, int> certificateQueriesRepository,
    IQueryRepository<VehicleCategory, int> vehicleQueriesRepository,
    IQueryRepository<Transmission, int> transmissionQueriesRepository,

    ISchoolCategoryPricingQueryRepository schoolCategoryPricingQueryRepository) : UnitOfWork(appDbContext), ISchoolManagementDirectory
{
    public ISchoolQueriesRepository SchoolQueriesRepository => schoolQueriesRepository;

    public ISchoolCommandRepository SchoolCommandRepository => schoolCommandRepository;

    public IQueryRepository<City, int> CityQueriesRepository => cityQueriesRepository;

    public IQueryRepository<ArrCertificate, int> CertificateQueriesRepository => certificateQueriesRepository;

    public IQueryRepository<VehicleCategory, int> VehicleQueriesRepository => vehicleQueriesRepository;

    public ISchoolCategoryPricingQueryRepository SchoolCategoryPricingQueriesRepository => schoolCategoryPricingQueryRepository;

    public IQueryRepository<Transmission, int> TransmissionQueriesRepository => transmissionQueriesRepository;
}
