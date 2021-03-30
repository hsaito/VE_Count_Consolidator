# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY . .

WORKDIR /source
RUN dotnet test
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/runtime:5.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "VECountConsolidatorCli.dll", "-m", "create"]
