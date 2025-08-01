# Synopsis
This project exists to learn about event streaming - in particular, how to orchestrate the ingestion of changes against SQL Server tables into an event system like Kafka so they can subsequently be processed by consumer applications.

# Requirements
- [WSL2](https://learn.microsoft.com/en-us/windows/wsl/install)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Microsoft ODBC Driver for SQL Server Version 17](https://learn.microsoft.com/en-us/sql/connect/odbc/download-odbc-driver-for-sql-server?view=sql-server-ver17#version-17)

# Setup
1. Run `docker-compose up` to create the Docker containers
1. After everything is initialized and running, cd to the `sql` folder, install Python packages, and run the data generation script:
    ```
    cd sql
    pip install -r requirements.txt
    python run.py
    ```
    This generates a number of files in the `seed` folder and a seed_master.bat file. You do not need to interact with these files directly.
1. To create the database, run `seed/create.bat`. To tear it down, run `seed/destroy.bat`