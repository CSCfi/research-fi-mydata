#!/bin/sh
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Devel1234 -d master -C -i /usr/src/sql/initialize-db.sql