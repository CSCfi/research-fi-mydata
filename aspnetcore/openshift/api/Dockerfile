# Builder
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app
# Copy csproj and restore as distinct layers
COPY aspnetcore/src/api/api.csproj ./
RUN dotnet restore
# Copy everything else and build
COPY aspnetcore/src/api/ ./
RUN dotnet publish -c Release -o out



# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "api.dll"]