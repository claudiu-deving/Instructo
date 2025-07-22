using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Domain.Dtos;
using Domain.Dtos.Image;
using Domain.Dtos.PhoneNumbers;
using Domain.Dtos.School;
using Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;

namespace Infrastructure.Data.Configurations;
internal class SchoolDetailConfiguration : IEntityTypeConfiguration<SchoolDetailReadDto>
{
    public void Configure(EntityTypeBuilder<SchoolDetailReadDto> builder)
    {

        builder.HasNoKey().ToView("SchoolDetails");

        builder.Property(x => x.ArrCertificates).HasConversion(x => JsonConvert.SerializeObject(x),
            x => JsonConvert.DeserializeObject<List<ArrCertificationDto>>(x)??new List<ArrCertificationDto>())
                            .Metadata.SetValueComparer(new ValueComparer<List<ArrCertificationDto>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()));

        builder.Property(x => x.VehicleCategories).HasConversion(x => JsonConvert.SerializeObject(x),
            x => JsonConvert.DeserializeObject<List<VehicleCategoryDto>>(x)??new List<VehicleCategoryDto>())
                            .Metadata.SetValueComparer(new ValueComparer<List<VehicleCategoryDto>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()));

        builder.Property(x => x.PhoneNumbersGroups).HasConversion(x => JsonConvert.SerializeObject(x),
            x => JsonConvert.DeserializeObject<IEnumerable<PhoneNumberGroupDto>>(x)??new List<PhoneNumberGroupDto>())
                            .Metadata.SetValueComparer(new ValueComparer<IEnumerable<PhoneNumberGroupDto>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()));

        builder.Property(x => x.Links).HasConversion(x => JsonConvert.SerializeObject(x),
            x => JsonConvert.DeserializeObject<WebsiteLinkRead[]>(x))
                            .Metadata.SetValueComparer(new ValueComparer<WebsiteLinkRead[]>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToArray()));

        builder.Property(x => x.BussinessHours).HasConversion(x => JsonConvert.SerializeObject(x),
            x => JsonConvert.DeserializeObject<BussinessHours>(x));

        builder.Property(x => x.IconData).HasConversion(x => JsonConvert.SerializeObject(x),
            x => JsonConvert.DeserializeObject<ImageReadDto>(x));

        builder.Property(x => x.Locations).HasConversion(x => JsonConvert.SerializeObject(x),
            x => JsonConvert.DeserializeObject<List<AddressDto>>(x)??new List<AddressDto>())
                            .Metadata.SetValueComparer(new ValueComparer<List<AddressDto>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()));

        builder.Property(x => x.CategoryPricings).HasConversion(x => JsonConvert.SerializeObject(x),
            x => JsonConvert.DeserializeObject<List<SchoolCategoryPricingDto>>(x)??new List<SchoolCategoryPricingDto>())
                            .Metadata.SetValueComparer(new ValueComparer<List<SchoolCategoryPricingDto>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()));

        builder.Property(x => x.Team).HasConversion(x => JsonConvert.SerializeObject(x),
            x => JsonConvert.DeserializeObject<TeamDto>(x));

        builder.Property(x => x.SchoolStatistics).HasConversion(x => JsonConvert.SerializeObject(x),
            x => JsonConvert.DeserializeObject<SchoolStatisticsDto>(x));

    }
}
