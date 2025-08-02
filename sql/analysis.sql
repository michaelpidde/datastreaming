select p.name as Product, c.company as Company, o.[count] as ProductCountInOrder, count(*) as [OrderCount]
from [order] o
inner join product p on o.productId = p.id
inner join customer c on o.customerId = c.id
--where c.active = 1 and p.active = 1
group by c.company, p.name, o.[count]
order by c.company, count(*) desc;
