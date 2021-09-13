# Mydata API application
This application implements API functionality for Researchfi Mydata.

# Environment variables
- ASPNETCORE_ENVIRONMENT=Development
- ASPNETCORE_URLS=http://*:8080
- ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
- ELASTICSEARCH__ENABLED=0
- CONNECTIONSTRINGS__DEFAULTCONNECTION
  - SQL Server connection string
  - "Server=<sql server address>;User Id=<user id>;Password=<password>;database=<db name>;TrustServerCertificate=True;Integrated Security=false;Trusted_Connection=false;"
- OAUTH__AUTHORITY
  - Identity server address
