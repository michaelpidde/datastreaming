if not exists(select * from sys.objects where object_id = object_id(N'.dbo.customer') and type = 'U') begin
	create table customer (
		id bigint identity(1,1) not null,
		company varchar(100) not null,
		active bit not null default 1,

		constraint pk_customer_id primary key clustered (id)
	)
end

if not exists(select * from sys.objects where object_id = object_id(N'.dbo.product') and type = 'U') begin
	create table product (
		id bigint identity(1,1) not null,
		[name] varchar(100) not null,
		[description] varchar(max) not null,
		msrp decimal(10,2) not null,
		active bit not null default 1,

		constraint pk_product_id primary key clustered (id)
	)
end

if not exists(select * from sys.objects where object_id = object_id(N'.dbo.inventory') and type = 'U') begin
    create table inventory (
        productId bigint unique not null,
        inventoryCount int not null default 0,

        constraint fk_inventory_productId foreign key (productId) references dbo.product(id) on delete cascade
    )
end

if not exists(select * from sys.objects where object_id = object_id(N'.dbo.order') and type = 'U') begin
	create table [order] (
		id bigint identity(1,1) not null,
		customerId bigint not null,
		productId bigint not null,
		created datetime not null default getdate(),
		fulfilled datetime default null,
		cancelled datetime default null,

		constraint pk_order_id primary key clustered (id),
		constraint fk_order_customerId foreign key (customerId) references dbo.customer(id),
		constraint fk_order_productId foreign key (productId) references dbo.product(id)
	)
end

if not exists(select * from sys.objects where object_id = object_id(N'.dbo.order_rollup') and type = 'U') begin
	create table order_rollup (
	    id bigint identity(1,1) not null,
		customerId bigint not null,
		productId bigint not null,
		openOrders int not null,
		fulfilledOrders int not null,
		agingDays int not null,

        constraint pk_order_rollup_id primary key clustered (id),
		constraint fk_order_rollup_customerId foreign key (customerId) references dbo.customer(id),
		constraint fk_order_rollup_productId foreign key (productId) references dbo.product(id)
	)
end

exec sys.sp_cdc_enable_db;
go

exec sys.sp_cdc_enable_table
    @source_schema = N'dbo',
    @source_name = N'customer',
    @role_name = NULL,
    @supports_net_changes = 1;
go

exec sys.sp_cdc_enable_table
    @source_schema = N'dbo',
    @source_name = N'product',
    @role_name = NULL,
    @supports_net_changes = 1;
go

exec sys.sp_cdc_enable_table
    @source_schema = N'dbo',
    @source_name = N'order',
    @role_name = NULL,
    @supports_net_changes = 1;
go
