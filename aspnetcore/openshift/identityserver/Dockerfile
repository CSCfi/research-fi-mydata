# Builder
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app
# Copy csproj and restore as distinct layers
COPY aspnetcore/src/identityserver/identityserver.csproj ./
RUN dotnet restore
# Copy everything else and build
COPY aspnetcore/src/identityserver/ ./
RUN dotnet publish -c Release -o out



# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "identityserver.dll"]
