global using System.Security.Claims;
global using System.Text;
global using System.Text.Json;

global using AspNetCoreRateLimit;

global using FluentValidation;

global using Instructo.Api.Endpoints;
global using Instructo.Api.Middleware;
global using Instructo.Application.Behaviors;
global using Instructo.Application.Users.Commands.RegisterUser;
global using Instructo.Domain.Entities;
global using Instructo.Domain.Interfaces;
global using Instructo.Domain.Shared;
global using Instructo.Infrastructure.Data;
global using Instructo.Infrastructure.Data.Configurations;
global using Instructo.Infrastructure.Data.Repositories;
global using Instructo.Infrastructure.Identity;

global using MediatR;

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Http.Json;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;

global using OpenTelemetry.Metrics;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;

global using Scalar.AspNetCore;

global using Serilog;
global using Serilog.Extensions.Hosting;