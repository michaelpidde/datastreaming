SELECT is_cdc_enabled FROM sys.databases WHERE name = 'application';
SELECT * FROM cdc.change_tables;
SELECT status_desc FROM sys.dm_server_services WHERE servicename LIKE '%Agent%';