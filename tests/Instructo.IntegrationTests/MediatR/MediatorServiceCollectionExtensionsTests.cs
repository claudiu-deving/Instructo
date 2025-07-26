using System.Reflection;

using Application.Abstractions.Messaging;
using Application.Schools.Commands.CreateSchool;
using Application.Schools.Commands.DeleteSchool;
using Application.Schools.Queries.GetSchools;
using Application.Users.Commands.ChangePassword;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.ForgotPassoword;
using Application.Users.Commands.LoginUser;
using Application.Users.Commands.RegisterUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Queries.GetUserByEmail;
using Application.Users.Queries.GetUsers;

using Domain.Dtos.School;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using Infrastructure.Data;
using Infrastructure.Data.Repositories.Commands;
using Infrastructure.Data.Repositories.Directories;
using Infrastructure.Data.Repositories.Queries;

using Messager;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace Instructo.IntegrationTests.MediatR;

public class MediatorServiceCollectionExtensionsTests : IDisposable
{
    private readonly ServiceProvider serviceProvider;
    private readonly ServiceCollection services;
    private readonly Assembly applicationAssembly;

    public MediatorServiceCollectionExtensionsTests()
    {
        services=new ServiceCollection();
        applicationAssembly=typeof(RegisterUserCommand).Assembly;
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("InstructoTestDb");
        });
        services.AddScoped(sp => new Mock<IIdentityService>().Object);
        services.AddScoped(sp => new Mock<ILogger<GetUserByEmailQueryHandler>>().Object);
        services.AddScoped(sp => new Mock<ILogger<GetUserByIdQueryHandler>>().Object);
        services.AddScoped(sp => new Mock<ILogger<GetUsersQueryHandler>>().Object);

        services.AddScoped(sp => new Mock<ILogger<DeleteUserByIdCommandHandler>>().Object);
        services.AddScoped(sp => new Mock<ILogger<UpdateUserCommandHandler>>().Object);
        services.AddScoped(sp => new Mock<ILogger<ChangePasswordCommandHandler>>().Object);
        services.AddScoped(sp => new Mock<ILogger<ForgotPasswordCommandHandler>>().Object);
        services.AddScoped(sp => new Mock<ILogger<LoginUserCommandHandler>>().Object);
        services.AddScoped(sp => new Mock<ILogger<RegisterUserCommandHandler>>().Object);

        services.AddScoped(sp => new Mock<ILogger<GetSchoolsQueryHandler>>().Object);
        services.AddScoped(sp => new Mock<ILogger<CreateSchoolCommandHandler>>().Object);
        services.AddScoped(sp => new Mock<ILogger<DeleteSchoolCommandHandler>>().Object);
        services.AddScoped(sp => new Mock<ILogger<RoleManager<ApplicationRole>>>().Object);
        services.AddScoped(sp => new Mock<ILogger<SchoolQueriesRepository>>().Object);
        services.AddScoped(sp => new Mock<ILogger<ImageCommandRepository>>().Object);





        services.AddScoped<ISchoolQueriesRepository, SchoolQueriesRepository>();
        services.AddScoped<IQueryRepository<VehicleCategory, int>, VehicleCategoryQueriesRepository>();
        services.AddScoped<IQueryRepository<ArrCertificate, int>, ArrCertificateQueriesRepository>();
        services.AddScoped<IQueryRepository<Transmission, int>, TransmissionQueryRepository>();
        services.AddScoped<IQueryRepository<Address, int>, AddressQueryRepository>();
        services.AddScoped<IInstructorProfileQueryRepository, InstructorProfileQueryRepository>();
        services.AddScoped<ISchoolCategoryPricingQueryRepository, SchoolCategoryPricingQueryRepository>();

        services.AddScoped<IQueryRepository<City, int>, CityQueryRepository>();
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit=true;
            options.Password.RequiredLength=8;
            options.Password.RequireNonAlphanumeric=true;
            options.Password.RequireUppercase=true;
            options.Password.RequireLowercase=true;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts=5;

            // User settings
            options.User.RequireUniqueEmail=true;
        })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

        services.AddScoped<ISchoolCommandRepository, SchoolCommandRepository>();
        services.AddScoped(sp => new Mock<ILogger<SchoolCommandRepository>>().Object);

        services.AddScoped<ICommandRepository<Image, ImageId>, ImageCommandRepository>();
        services.AddScoped<ISchoolManagementDirectory, SchoolManagementDirectory>();
        services.AddSingleton<ISocialMediaPlatformImageProvider, SocialMediaPlatformImageProvider>();
        services.AddTransient<IUserQueriesRepository, UserQueryRepository>();
        services.AddMediator(applicationAssembly);

        serviceProvider=services.BuildServiceProvider();
    }

    [Fact]
    public void AddMediatR_WithAssemblyParameter_ShouldRegisterMediatorServices()
    {
        Assert.NotNull(serviceProvider.GetService<IMediator>());
        Assert.NotNull(serviceProvider.GetService<ISender>());
        Assert.NotNull(serviceProvider.GetService<IPublisher>());
    }

    [Fact]
    public void AddMediatR_WithConfiguration_ShouldRegisterMediatorServices()
    {
        Assert.NotNull(serviceProvider.GetService<IMediator>());
        Assert.NotNull(serviceProvider.GetService<ISender>());
        Assert.NotNull(serviceProvider.GetService<IPublisher>());
    }

    [Fact]
    public void AddMediatR_ShouldRegisterCommandHandlers()
    {
        var registerUserHandler = serviceProvider.GetService<ICommandHandler<RegisterUserCommand, Domain.Shared.Result<Domain.Entities.ApplicationUser>>>();
        Assert.NotNull(registerUserHandler);
        Assert.IsType<RegisterUserCommandHandler>(registerUserHandler);
    }

    [Fact]
    public void AddMediatR_ShouldRegisterQueryHandlers()
    {

        var getUserHandler = serviceProvider.GetService<ICommandHandler<GetUserByEmailQuery, Domain.Shared.Result<Domain.Entities.ApplicationUser>>>();
        Assert.NotNull(getUserHandler);
        Assert.IsType<GetUserByEmailQueryHandler>(getUserHandler);
    }

    [Fact]
    public void AddMediatR_ShouldRegisterAllHandlersFromAssembly()
    {
        var handlerTypes = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Handler")&&!t.IsAbstract&&!t.IsInterface)
            .ToList();

        Assert.True(handlerTypes.Count>0, "No handlers found in assembly");

        foreach(var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType&&
                           (i.GetGenericTypeDefinition()==typeof(IRequestHandler<,>)||
                            i.GetGenericTypeDefinition()==typeof(IRequestHandler<>)))
                .ToList();

            foreach(var interfaceType in interfaces)
            {
                var handler = serviceProvider.GetService(interfaceType);
                Assert.NotNull(handler);
                Assert.Equal(handlerType, handler.GetType());
            }
        }
    }

    [Fact]
    public void AddMediatR_WithMultipleAssemblies_ShouldRegisterAllHandlers()
    {
        Assert.NotNull(serviceProvider.GetService<IMediator>());
        Assert.NotNull(serviceProvider.GetService<ISender>());
        Assert.NotNull(serviceProvider.GetService<IPublisher>());

        var registerUserHandler = serviceProvider.GetService<ICommandHandler<RegisterUserCommand, Domain.Shared.Result<Domain.Entities.ApplicationUser>>>();
        Assert.NotNull(registerUserHandler);
    }

    [Fact]
    public void AddMediatR_WithConfigurationMultipleAssemblies_ShouldRegisterAllHandlers()
    {
        Assert.NotNull(serviceProvider.GetService<IMediator>());
        var registerUserHandler = serviceProvider.GetService<ICommandHandler<RegisterUserCommand, Domain.Shared.Result<Domain.Entities.ApplicationUser>>>();
        Assert.NotNull(registerUserHandler);
    }

    [Fact]
    public void AddMediatR_ShouldRegisterServicesAsTransient()
    {
        var handlerDescriptors = services.Where(s => s.ServiceType.IsGenericType&&
                                                    (s.ServiceType.GetGenericTypeDefinition()==typeof(IRequestHandler<,>)||
                                                     s.ServiceType.GetGenericTypeDefinition()==typeof(IRequestHandler<>)))
                                          .ToList();

        Assert.All(handlerDescriptors, descriptor => Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime));
    }

    [Fact]
    public void AddMediatR_ShouldRegisterMediatorServicesAsScoped()
    {
        var mediatorDescriptor = services.First(s => s.ServiceType==typeof(IMediator));
        var senderDescriptor = services.First(s => s.ServiceType==typeof(ISender));
        var publisherDescriptor = services.First(s => s.ServiceType==typeof(IPublisher));

        Assert.Equal(ServiceLifetime.Scoped, mediatorDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, senderDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, publisherDescriptor.Lifetime);
    }

    [Fact]
    public void AddMediatR_ShouldNotRegisterAbstractOrInterfaceTypes()
    {
        var allHandlerTypes = applicationAssembly.GetTypes()
            .Where(t => (t.IsAbstract||t.IsInterface)&&t.Name.Contains("Handler"))
            .ToList();

        foreach(var abstractType in allHandlerTypes)
        {
            var interfaces = abstractType.GetInterfaces()
                .Where(i => i.IsGenericType&&
                           (i.GetGenericTypeDefinition()==typeof(IRequestHandler<,>)||
                            i.GetGenericTypeDefinition()==typeof(IRequestHandler<>)))
                .ToList();

            foreach(var interfaceType in interfaces)
            {
                var handler = serviceProvider.GetService(interfaceType);
                if(handler!=null)
                {
                    Assert.False(handler.GetType().IsAbstract);
                    Assert.False(handler.GetType().IsInterface);
                }
            }
        }
    }

    [Theory]
    [InlineData(typeof(RegisterUserCommand))]
    [InlineData(typeof(GetUserByEmailQuery))]
    public void AddMediatR_ShouldRegisterSpecificHandlerTypes(Type requestType)
    {
        var handlerTypes = applicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Handler")&&!t.IsAbstract&&!t.IsInterface)
            .Where(t => t.Name.Contains(requestType.Name.Replace("Command", "").Replace("Query", "")))
            .ToList();

        Assert.True(handlerTypes.Count>0, $"No handlers found for {requestType.Name}");

        foreach(var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType&&
                           (i.GetGenericTypeDefinition()==typeof(IRequestHandler<,>)||
                            i.GetGenericTypeDefinition()==typeof(IRequestHandler<>)))
                .ToList();

            foreach(var interfaceType in interfaces)
            {
                var handler = serviceProvider.GetService(interfaceType);
                Assert.NotNull(handler);
                Assert.Equal(handlerType, handler.GetType());
            }
        }
    }

    //Cleanup
    public void Dispose()
    {
        var service = serviceProvider.GetService<IRequestHandler<GetSchoolsQuery, Result<IEnumerable<ISchoolReadDto>>>>();
        Assert.NotNull(service);
        if(serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

    }
}