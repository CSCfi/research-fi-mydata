# Builder
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
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
RUN chmod a+rwx *
EXPOSE 8080
ENTRYPOINT ["dotnet", "identityserver.dll"]
