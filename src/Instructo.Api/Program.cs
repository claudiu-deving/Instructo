using System;
using System.Text;

using AutoMapper;

using Instructo.Domain.Interfaces;
using Instructo.Infrastructure.Data;
using Instructo.Infrastructure.Data.Configurations;
using Instructo.Infrastructure.Data.Repositories;
using Instructo.Infrastructure.Identity;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Instructo.Api.Middleware;
using Scalar.AspNetCore;
using Instructo.Api.Endpoints;
using Instructo.Domain.Entities;
using MediatR;
using Instructo.Application.Behaviors;
using FluentValidation;
using Instructo.Domain.Shared;
using Instructo.Infrastructure.Migrations;
using Serilog;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog.Extensions.Hosting;
using System.Security.Claims;
using AspNetCoreRateLimit;
using Microsoft.Extensions.DependencyInjection;
using Instructo.Application.Users.Query;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Instructo.Application.Users.Commands.RegisterUser;

var builder = WebApplication.CreateBuilder(args);



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
        throw new ArgumentNullException("Provide the url for the seq server"));
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
                options.EnrichWithHttpResponse=(activity, httpResponse) =>
                {
                    // Add custom response enrichment if needed
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
            })
            .AddOtlpExporter(options =>
            {
                options.Endpoint=new Uri(builder.Configuration["OpenTelemetry:OtlpEndpoint"]);
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
builder.Services.AddSingleton<IDiagnosticContext>(sp => sp.GetRequiredService<DiagnosticContext>());
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration["DefaultConnection"];

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});
builder.Services.AddTransient<IUserQueries>(p => new UserQueries(connectionString));

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
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


// Add authorization policies
builder.Services.AddAuthorizationBuilder()
    // Add authorization policies
    .AddPolicy("IronMan", policy => policy.RequireRole("IronMan").RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "13073b69-393f-4c5b-b96c-26b5d544d69e"))
    // Add authorization policies
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    // Add authorization policies
    .AddPolicy("SchoolOwners", policy => policy.RequireRole("Admin", "SchoolOwner"));

builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);

builder.Services.AddScoped<IUserRepository, UserRepository>();
var app = builder.Build();


//await DbInitializer.SeedRolesAndAdminUser(app.Services);
app.MapUserEndpoints();
app.MapAuthEndpoints();

if(app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
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
            diagnosticContext.Set("UserId", httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
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
