using Api;
using Api.Endpoints;
using Api.Middleware;

using Application.Behaviors;
using Application.Users.Commands.RegisterUser;

using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

using Infrastructure.Data;
using Infrastructure.Data.Configurations;
using Infrastructure.Data.Repositories.Commands;
using Infrastructure.Data.Repositories.Queries;
using Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
         .Destructure.With<SensitiveDataDestructuringPolicy>()
        .Enrich.WithThreadId()
        .WriteTo.Console()
        .WriteTo.Seq(context.Configuration["Seq:ServerUrl"]??
        throw new ArgumentException("Provide the url for the seq server"));
});

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource(builder.Environment.ApplicationName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName: builder.Environment.ApplicationName))
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException=true;
                options.EnrichWithHttpRequest=(activity, httpRequest) =>
                {
                    activity.SetTag("http.request.headers.correlation_id",
                        httpRequest.Headers["X-Correlation-ID"].FirstOrDefault()??"");
                };
            })
            .AddHttpClientInstrumentation(options =>
            {
                options.RecordException=true;
                options.EnrichWithException=(activity, exception) =>
                {
                    activity.SetTag("error.type", exception.GetType().Name);
                    activity.SetTag("error.message", exception.Message);
                };
            });
    })
    .WithMetrics(metricsProviderBuilder =>
    {
        metricsProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint=new Uri(builder.Configuration["OpenTelemetry:OtlpEndpoint"]);
            });
    });

// Integration between Serilog and OpenTelemetry
builder.Services.AddSingleton<IDiagnosticContext>(sp =>
sp.GetRequiredService<DiagnosticContext>());
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration["DefaultConnection"]??
    throw new ArgumentException("{DefaultConnection} is null, provide a valid DB Connection");

builder.Services.AddDbContext<IAppDbContext,AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
    if(builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
    else
    {
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
});
builder.Services.AddTransient<IUserQueries>(p => new UserQueryRepository(connectionString));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
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

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters=new TokenValidationParameters
    {
        ValidateIssuer=true,
        ValidateAudience=true,
        ValidateLifetime=true,
        ValidateIssuerSigningKey=true,
        ValidIssuer=jwtSettings.Issuer,
        ValidAudience=jwtSettings.Audience,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
    };

});


builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
});

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
builder.Services.AddValidatorsFromAssembly(
    typeof(RegisterUserCommand).Assembly, includeInternalTypes: true);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy=JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive=true;
});

// Register Identity services
builder.Services.AddScoped<IIdentityService, IdentityService>();


builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(
    builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(
    builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();



var ironManId = StartupHelpers.GetIronManId(args);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(ApplicationRole.IronMan.Name!, policy => policy.RequireRole(
            ApplicationRole.IronMan.Name!)
    .RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
        ironManId))
    .AddPolicy("AdminOnly", policy => policy.RequireRole(
        ApplicationRole.Admin.Name!, 
        ApplicationRole.Owner.Name!, 
        ApplicationRole.IronMan.Name!))
    .AddPolicy("SchoolOwners", policy => policy.RequireRole( 
        ApplicationRole.Owner.Name!));

builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddSingleton<IDbConnectionProvider>(
    new DbConnectionProvider(connectionString));
builder.Services.AddScoped<IQueryRepository<School, SchoolId>, SchoolQueriesRepository>();
builder.Services.AddScoped<IQueryRepository<VehicleCategory, VehicleCategoryType>, VehicleCategoryQueriesRepository>();
builder.Services.AddScoped<IQueryRepository<ArrCertificate, ARRCertificateType>, ArrCertificateQueriesRepository>();
builder.Services.AddScoped<ICommandRepository<School, SchoolId>, SchoolCommandRepository>();
builder.Services.AddScoped<ICommandRepository<Image, ImageId>, ImageCommandRepository>();
builder.Services.AddSingleton<ISocialMediaPlatformImageProvider, SocialMediaPlatformImageProvider>();
var app = builder.Build();


//await DbInitializer.SeedRolesAndAdminUser(app.Services);
app.MapUserEndpoints();
app.MapSchoolEndpoints();
app.MapAuthEndpoints();


if(app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}
app.UseExceptionHandler(exceptionHandlerApp
    => exceptionHandlerApp.Run(async context
        => await Results.Problem().ExecuteAsync(context)));

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext=(diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());

        if(httpContext.User?.Identity?.IsAuthenticated==true)
        {
            diagnosticContext.Set("UserId",
                httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    };
});

// Add your correlation ID middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<IronmanAccessLoggingMiddleware>();
app.UseRoleBasedAuthorization();
app.UseIpRateLimiting();

app.Run();
