@echo off

echo Creating database
sqlcmd -S tcp:localhost,1433 -U sa -P SAPassword! -d master -i "./database.sql"

echo Creating schema
sqlcmd -S tcp:localhost,1433 -U sa -P SAPassword! -d application -i "./schema.sql"

echo Seeding - this will take a few minutes
seed_master.bat

echo Done