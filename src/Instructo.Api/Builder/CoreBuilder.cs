using Api.Middleware;

using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;

using Infrastructure.Data;
using Infrastructure.Identity;

namespace Api.Builder;

public static class CoreBuilder
{
    public static void AddLogging(this WebApplicationBuilder builder)
    {
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
                .WriteTo.Seq(context.Configuration["Seq:ServerUrl"]?? //Make sure to run Docker beforehand
                             throw new ArgumentException("Provide the url for the seq server"));
        });


        // Integration between Serilog and OpenTelemetry
        builder.Services.AddSingleton<IDiagnosticContext>(sp =>
            sp.GetRequiredService<DiagnosticContext>());
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
    }

    public static void AddObservability(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource(builder.Environment.ApplicationName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(builder.Environment.ApplicationName))
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
                options.Endpoint=new Uri(builder.Configuration["OpenTelemetry:OtlpEndpoint"]??
                                           throw new ArgumentException("Provide the url for the OTLP endpoint"));
            });
    });

    }


    public static void AddAuthentification(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IIdentityService, IdentityService>();
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
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()??
                          throw new ArgumentException("JwtSettings is null, provide a valid JwtSettings");



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
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });
    }


    public static void AddAuthorization(this WebApplicationBuilder builder, string[] args)
    {
        var ironManId = GetIronManId();

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


        string GetIronManId()
        {
#if DEBUG
            return "8fb090d2-ad97-41d0-86ce-08ddc4a5a731";
#endif
            var s = Environment.GetEnvironmentVariable("IRONMAN_ID");
            if(s is not null)
                return s;
            if(args.Length>0&&args[0] is { } id)
                s=id;
            else
                throw new ArgumentException("{IRONMAN_ID} is missing,cannot proceed");

            return s;
        }
    }
}