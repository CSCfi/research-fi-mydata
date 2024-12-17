#!/bin/bash

# Enable job control. Needed to put SQL server process temporarily in the background
set -m

# Start SQL server process in the background
/opt/mssql/bin/sqlservr &

# Initialize database
sleep 10;
/usr/src/sql/initialize-db.sh

# Bring background SQL server process froto foreground to keep container running
fg %1