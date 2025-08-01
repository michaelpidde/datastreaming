if not exists(select * from sys.databases where name = N'application') begin
	create database application
end