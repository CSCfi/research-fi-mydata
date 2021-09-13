# Mydata Identityserver4 application
This application implements Identityserver4 functionality for Researchfi Mydata.

# Environment variables
- ASPNETCORE_ENVIRONMENT=Development
- ASPNETCORE_URLS=http://*:8080
- ORCID__CLIENTID=<ORCID client id>
- ORCID__CLIENTSECRET=<ORCID client secret>
- CONNECTIONSTRINGS__DEFAULTCONNECTION="Server=<sql server address>;User Id=<user id>;Password=<password>;database=<db name>;TrustServerCertificate=True;Integrated Security=false;Trusted_Connection=false;"
- IDENTITYSERVER__ORIGIN=<domain name of this server>
- JAVASCRIPTCLIENT__REDIRECTURI=<Javascript client redirect URI>
- JAVASCRIPTCLIENT__POSTLOGOUTREDIRECTURI=<Javascript client post logout redirect URI>
- JAVASCRIPTCLIENT__ALLOWEDCORSORIGIN=<Javascript client allowed origin>
