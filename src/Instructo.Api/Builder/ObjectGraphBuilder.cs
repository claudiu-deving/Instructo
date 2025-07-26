using Api.Builder;

using Application.Behaviors;
using Application.Users.Commands.RegisterUser;

using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueObjects;

using Infrastructure.Data.Repositories.Commands;
using Infrastructure.Data.Repositories.Directories;
using Infrastructure.Data.Repositories.Queries;

namespace Api.Builder;

public static class ObjectGraphBuilder
{
    public static void AddMediator(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediator(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
        });

        builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
    }

    public static void AddValidators(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(
            typeof(RegisterUserCommand).Assembly, includeInternalTypes: true);
    }

    public static void AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling=Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling=Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy=JsonNamingPolicy.CamelCase;
            options.SerializerOptions.PropertyNameCaseInsensitive=true;
        });
    }


    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IUserQueriesRepository, UserQueryRepository>();
        builder.Services.AddScoped<ISchoolQueriesRepository, SchoolQueriesRepository>();
        builder.Services.AddScoped<IQueryRepository<VehicleCategory, int>, VehicleCategoryQueriesRepository>();
        builder.Services.AddScoped<IQueryRepository<ArrCertificate, int>, ArrCertificateQueriesRepository>();
        builder.Services.AddScoped<IQueryRepository<City, int>, CityQueryRepository>();
        builder.Services.AddScoped<ISchoolCommandRepository, SchoolCommandRepository>();
        builder.Services.AddScoped<ISchoolManagementDirectory, SchoolManagementDirectory>();
        builder.Services.AddSingleton<ISocialMediaPlatformImageProvider, SocialMediaPlatformImageProvider>();
        builder.Services.AddScoped<ISchoolCategoryPricingQueryRepository, SchoolCategoryPricingQueryRepository>();
        builder.Services.AddScoped<IQueryRepository<Transmission, int>, TransmissionQueryRepository>();
        builder.Services.AddScoped<IQueryRepository<Address, int>, AddressQueryRepository>();
        builder.Services.AddScoped<IInstructorProfileQueryRepository, InstructorProfileQueryRepository>();
        builder.Services.AddScoped<IImagesQueryRepository, ImagesQueryRepository>();
    }

}
