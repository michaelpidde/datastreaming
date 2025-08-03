@echo off

echo Creating database
sqlcmd -S tcp:localhost,1433 -U sa -P SAPassword! -d master -i "./database.sql"

echo Creating schema
sqlcmd -S tcp:localhost,1433 -U sa -P SAPassword! -d application -i "./schema.sql"

echo Seeding - this will take a few minutes
call seed_master.bat

echo Seeding rollup tables
sqlcmd -S tcp:localhost,1433 -U sa -P SAPassword! -d application -i "./seed_rollups.sql"

echo Done
