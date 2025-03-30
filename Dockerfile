FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Instructo.Api/Instructo.Api.csproj", "Instructo.Api/"]
COPY ["Instructo.Application/Instructo.Application.csproj", "Instructo.Application/"]
COPY ["Instructo.Domain/Instructo.Domain.csproj", "Instructo.Domain/"]
COPY ["Instructo.Infrastructure/Instructo.Infrastructure.csproj", "Instructo.Infrastructure/"]
RUN dotnet restore "Instructo.Api/Instructo.Api.csproj"
COPY . .
WORKDIR "/src/Instructo.Api"
RUN dotnet build "Instructo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Instructo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Instructo.Api.dll"]