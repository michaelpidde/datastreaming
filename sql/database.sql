if not exists(select * from sys.databases where name = N'application') begin
	create database application
end

-- Enable advanced options
exec sp_configure 'show advanced options', 1;
reconfigure;

-- Enable Agent XPs
exec sp_configure 'Agent XPs', 1;
reconfigure;