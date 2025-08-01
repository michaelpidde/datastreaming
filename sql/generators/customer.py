from faker import Faker

fake = Faker()

inserts = []
for i in range(1000):
    company = fake.company().replace("'", "''")  # Escape single quotes
    if i == 999:
        inserts.append(f"('{company}')")
    else:
        inserts.append(f"('{company}'),")

print("Generating seed/customer.sql")
with open("./seed/customer.sql", "w", encoding="utf-8") as file:
    file.write("set nocount on\n")
    file.write("INSERT INTO customer (company) VALUES\n\t")
    file.write("\n\t".join(inserts))

with open("./seed_master.bat", "a", encoding="utf-8") as master:
    master.write(f"sqlcmd -S tcp:localhost,1433 -U sa -P SAPassword! -d application -m -1 -i \"./seed/customer.sql\"\n")