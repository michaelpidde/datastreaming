import decimal
import random

from faker import Faker
import factory # factory_boy
import pprint #prettyprint

fake = Faker()

class Product:
    product = ""
    description = ""
    price = 0

    def __init__(self, product, description, price):
        self.product = product
        self.description = description
        self.price = price
    
    def __repr__(self):
        return pprint.pformat(vars(self), indent=4, width=1)

class ProductFactory(factory.Factory):
    class Meta:
        model = Product
    
    product = factory.Faker('sentence', nb_words=4)
    description = factory.Faker('sentence', nb_words=50)
    price = 0

inserts = []
for i in range(5000):
    product = ProductFactory()
    product.price = decimal.Decimal(random.randrange(10000))/100
    inserts.append(f"('{product.product}', '{product.description}', {product.price}),")

chunk_size = 1000
print("Generating seed/product.sql")
with open("./seed/product.sql", "w", encoding="utf-8") as file:
    file.write("set nocount on\n")
    for i in range(0, len(inserts), chunk_size):
        chunk = inserts[i:i + chunk_size]
        chunk[-1] = chunk[-1][:-1]
        file.write("INSERT INTO product ([name], [description], msrp) VALUES\n\t")
        file.write("\n\t".join(chunk))
        file.write(";\n\n")

with open("./seed_master.bat", "a", encoding="utf-8") as master:
    master.write(f"sqlcmd -S tcp:localhost,1433 -U sa -P SAPassword! -d application -m -1 -i \"./seed/product.sql\"\n")