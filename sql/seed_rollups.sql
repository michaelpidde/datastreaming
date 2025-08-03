set nocount on

if not exists(select top 1 * from order_rollup) begin
	insert into order_rollup (customerId, productId, openOrders, openOrdersInventoryCount, fulfilledOrders, agingDays)
	select c.id as customerId, p.id as productId, count(*) as openOrders, sum(o.[count]) as openOrdersInventoryCount, 0, 0
	from [order] o
	inner join product p on o.productId = p.id
	inner join customer c on o.customerId = c.id
	where c.active = 1 and p.active = 1 and o.fulfilled is null and o.cancelled is null
	group by c.id, p.id
	--order by c.id, p.id
end

if not exists(select top 1 * from inventory_rollup) begin
	insert into inventory_rollup (productId, openOrdersInventoryCount, totalFulfilledOrdersInventoryCount)
	select p.id, sum(o.[count]), 0
	from [order] o
	inner join product p on o.productId = p.id
	where p.active = 1 and o.fulfilled is null and o.cancelled is null
	group by p.id
	--order by sum(o.[count]) desc
end
