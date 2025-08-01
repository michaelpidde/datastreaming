select p.name as Product, c.company as Company, count(*) as [Count]
from [order] o
inner join product p on o.productId = p.id
inner join customer c on o.customerId = c.id
--where c.active = 1 and p.active = 1
group by c.company, p.name
order by count(*) desc;

select count(*) from [order];
