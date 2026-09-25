# Builder
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app
# Copy csproj and restore as distinct layers
COPY aspnetcore/custom_nuget/ /app/custom_nuget
COPY aspnetcore/nuget.config ./
COPY aspnetcore/src/api/api.csproj ./api/
COPY aspnetcore/src/api.PublicApiContracts/api.PublicApiContracts.csproj ./api.PublicApiContracts/
RUN dotnet restore ./api/api.csproj
# Copy everything else and build
COPY aspnetcore/src/api/ ./api/
COPY aspnetcore/src/api.PublicApiContracts/ ./api.PublicApiContracts/
RUN dotnet publish ./api/api.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
ENV TZ=Europe/Helsinki
COPY --from=build-env /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "api.dll"]
