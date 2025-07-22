using Domain.Entities;

using Infrastructure.Data.Repositories.Queries;

namespace Domain.Interfaces;
public interface ISchoolManagementDirectory : IUnitOfWork
{
    ISchoolQueriesRepository SchoolQueriesRepository { get; }
    ISchoolCommandRepository SchoolCommandRepository { get; }
    IQueryRepository<City, int> CityQueriesRepository { get; }
    IQueryRepository<ArrCertificate, int> CertificateQueriesRepository { get; }
    IQueryRepository<VehicleCategory, int> VehicleQueriesRepository { get; }
    IQueryRepository<Transmission, int> TransmissionQueriesRepository { get; }
    ISchoolCategoryPricingQueryRepository SchoolCategoryPricingQueriesRepository { get; }
}
