# Mydata API application
This application implements API functionality for Researchfi Mydata.

# Environment variables

Following table lists the environment variable required by the application.
Notice that the number of underscores ("_" vs "__") in the variable name is significant!

| Name | Purpose | Example |
| :--- | :---    | :---    |
| ADMINTOKEN | Token which should be used in admin api calls | 09c349d5c09c380f370fc5e02c87fb11 |
| ASPNETCORE_ENVIRONMENT | ASP.NET Core uses an environment variable called ASPNETCORE_ENVIRONMENT to indicate the runtime environment. | "Development" or "Production" |
| ASPNETCORE_FORWARDEDHEADERS_ENABLED | https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer | true |
| ASPNETCORE_URLS | Indicates the IP addresses or host addresses with ports and protocols that the server should listen on for requests. | http://*:8080 |
| CONNECTIONSTRINGS__DEFAULTCONNECTION | Database connection string | Server=testdatabase.mydomain.com;User Id=TestDbUser;Password=f39JWDHFHD329ca02ds7f3f;database=test_db;TrustServerCertificate=True;Integrated Security=false;Trusted_Connection=false; |
| ELASTICSEARCH__ENABLED | Toggle Elasticsearch integration | true |
| ELASTICSEARCH__PASSWORD | HTTP basic auth password for Elasticsearch network connection | d983d98hsdf98 |
| ELASTICSEARCH__URL | URL where Elasticsearch is accessible | https://testelastic.mydomain.com/ |
| ELASTICSEARCH__USERNAME | HTTP basic auth username for Elasticsearch network connection | elasticsearchusername |
| KEYCLOAK__ORCIDTOKENENDPOINT | Keycloak endpoint for retrieving user's external IDP token | https://testkeycloak.mydomain.com/realms/mydata/broker/orcid/token |
| KEYCLOAK__REALM | Keycloak realm | https://testkeycloak.mydomain.com/realms/mydata |
| KEYCLOAK__TOKENENDPOINT | Keycloak Admin user token management | https://testkeycloak.mydomain.com/realms/mydata/protocol/openid-connect/token |
| KEYCLOAK__ADMIN__CLIENTID | Keycloak Admin user token management | mydata-api |
| KEYCLOAK__ADMIN__CLIENTSECRET | Keycloak Admin user token management | 67df95be76ce5ddfa02991af5cf1c691 |
| KEYCLOAK__ADMIN__REALMUSERSENDPOINT | Keycloak Admin API - user management | https://testkeycloak.mydomain.com/admin/realms/mydata/users/ |
| LOGSERVER_PASSWORD | HTTP basic auth password for log server network connection | logtestuser |
| LOGSERVER_USERNAME | HTTP basic auth username for log server network connection | 339753df8a6a6c0b31b43a3d1964b6f6 |
| ORCID__CLIENTID | ORCID member API client ID | APP-1073b1f208fcb2fe67ae669899d3ea91 |
| ORCID__CLIENTSECRET | ORCID member API client secret | f9fbda7183c930015fddbe34baf0e9dab4efffae708aaa8fce1834c2fb4bf537 |
| ORCID__MEMBERAPI | ORCID member API endpoint | https://api.sandbox.orcid.org/v3.0/ |
| ORCID__PUBLICAPI | ORCID public API endpoint | https://pub.orcid.org/v3.0/ |
| ORCID__WEBHOOK__ACCESSTOKEN | ORCID webhook access token | 184bbfdc185ce506 |
| ORCID__WEBHOOK__API | ORCID webhook API endpoint | https://api.sandbox.orcid.org/ |
| ORCID__WEBHOOK__ENABLED | Toggle ORCID webhook integration  | true |
| SERILOG:WRITETO:HTTPSINK:ARGS:REQUESTURI |  |
| SERVICEURL | Publicly visible domain of the application | https://mydata-api-devel.mydomain.com
