using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;
public interface ISchoolManagementDirectory : IUnitOfWork
{
    ISchoolQueriesRepository SchoolQueriesRepository { get; }
    ISchoolCommandRepository SchoolCommandRepository { get; }
    IQueryRepository<City, int> CityQueriesRepository { get; }
    IQueryRepository<ArrCertificate, ARRCertificateType> CertificateQueriesRepository { get; }
    IQueryRepository<VehicleCategory, VehicleCategoryType> VehicleQueriesRepository { get; }
}
