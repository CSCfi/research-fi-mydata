# Mydata API application
This application implements API functionality for Researchfi Mydata.

# Environment variables

Following table lists the environment variables used by the application.
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
| LOGSERVER__PASSWORD | HTTP basic auth password for log server network connection | `<password>` |
| LOGSERVER__USERNAME | HTTP basic auth username for log server network connection | `<username>` |
| OPENAIOPTIONS__ALLOWSELFSIGNEDCERTIFICATE | Optional. Allow the OpenAI API client to accept self-signed certificates. Defaults to false. | false |
| OPENAIOPTIONS__APIKEY | API key used by AI features | `<api-key>` |
| OPENAIOPTIONS__BASEURL | Base URL for the OpenAI-compatible API used by AI features | https://api.example.com/v1 |
| OPENAIOPTIONS__MODEL | Model name used by AI features | `<model-name>` |
| ORCID__CLIENTID | ORCID member API client ID | APP-1073b1f208fcb2fe67ae669899d3ea91 |
| ORCID__CLIENTSECRET | ORCID member API client secret | f9fbda7183c930015fddbe34baf0e9dab4efffae708aaa8fce1834c2fb4bf537 |
| ORCID__MEMBERAPI | ORCID member API endpoint | https://api.sandbox.orcid.org/v3.0/ |
| ORCID__PUBLICAPI | ORCID public API endpoint | https://pub.orcid.org/v3.0/ |
| ORCID__WEBHOOK__ACCESSTOKEN | ORCID webhook access token | 184bbfdc185ce506 |
| ORCID__WEBHOOK__API | ORCID webhook API endpoint | https://api.sandbox.orcid.org/ |
| ORCID__WEBHOOK__ENABLED | Toggle ORCID webhook integration  | true |
| PORTAL__URL__EN | URL of the English-language frontend portal | https://research.fi/en/ |
| PORTAL__URL__FI | URL of the Finnish-language frontend portal | https://tiedejatutkimus.fi/fi/ |
| PORTAL__URL__SV | URL of the Swedish-language frontend portal | https://forskning.fi/sv/ |
| PUBLICAPITOKEN | Shared-secret token required in the `publicapitoken` request header for the Public API integration endpoint (`POST /api/publicapi/profile`) | 184bbfdc185ce506 |
| RUNTIMESTATUSLOGGING__INTERVALSECONDS | Optional. Interval for periodic runtime status log entries (memory, CPU, background queue length, etc.). Defaults to 300 if unset. Set to 0 or a negative value to disable. | 300 |
| SERILOG__WRITETO__HTTPSINK__ARGS__REQUESTURI | URL where the HTTP log sink sends log events | https://logs.example.com/ |
| SERVICEURL | Publicly visible domain of the application | https://mydata-api-devel.mydomain.com

## Public API profile endpoint

`POST /api/publicapi/profile` requires the configured `PUBLICAPITOKEN` in the `publicapitoken`
header. Requests identify a person with one or two `ORCID`/`HETU` identifiers and a bare ROR ID
in `controllerROR`. ORCID takes precedence over HETU. Profile data is returned only when the
profile is visible and an active consent exists for the registered controller; hidden profiles
are recognized but their profile data is withheld. The consent-management URL for each language
is built from `PORTAL__URL__FI`, `PORTAL__URL__EN`, or `PORTAL__URL__SV`, followed by
`/mydata/account-settings?login=1`.
