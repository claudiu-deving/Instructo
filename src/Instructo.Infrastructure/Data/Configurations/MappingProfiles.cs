using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Instructo.Domain.Entities;
using Instructo.Domain.ValueObjects;

namespace Instructo.Infrastructure.Data.Configurations;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Domain to Entity mappings
        CreateBaseAuditableMap<School, SchoolEntity>();


        // Entity to Domain mappings
        CreateMap<SchoolEntity, School>();

        // Domain to Entity mappings
        CreateMap<User, ApplicationUser>()
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
             .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore());
        // Entity to Domain mappings
        CreateMap<ApplicationUser, User>();
    }

    private IMappingExpression<T1, T2> CreateBaseAuditableMap<T1, T2>() where T2 : BaseAuditableEntity
    {
        return CreateMap<T1, T2>()
             .ForMember(dest => dest.Created, opt => opt.Ignore())
             .ForMember(dest => dest.LastModified, opt => opt.Ignore())
             .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
             .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore());
    }
}
