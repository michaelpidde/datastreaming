#!/bin/bash
# Enable SQL Server Agent (needed in order to use CDC)
/opt/mssql/bin/mssql-conf set sqlagent.enabled true

# Start SQL Server
/opt/mssql/bin/sqlservr
