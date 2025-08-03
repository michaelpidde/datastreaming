import random

inserts = []
# 500,0000
print("Generating random order data")
for i in range(1000000):
    customerId = random.randint(1, 1000)
    productId = random.randint(1, 5000)
    productCount = random.randint(1, 20)
    inserts.append(f"({customerId}, {productId}, {productCount}),")

chunk_size = 1000
file_max_inserts = 100000
i = 0
file_index = 1

while i < len(inserts):
    print(f"Generating seed/order_{file_index}.sql")
    with open(f"./seed/order_{file_index}.sql", "w", encoding="utf-8") as file:
        file.write("set nocount on\n")
        inserts_in_file = 0
        while inserts_in_file < file_max_inserts and i < len(inserts):
            chunk = inserts[i:i + chunk_size]
            i += chunk_size
            inserts_in_file += len(chunk)
            chunk[-1] = chunk[-1][:-1] # remove trailing comma from last element in chunk
            file.write("INSERT INTO [order] (customerId, productId, [count]) VALUES ")
            file.write("".join(chunk))
            file.write(";\n\n")

    with open("./seed_master.bat", "a", encoding="utf-8") as master:
        master.write(f"sqlcmd -S tcp:localhost,1433 -U sa -P SAPassword! -d application -m -1 -i \"./seed/order_{file_index}.sql\"\n")
    
    file_index += 1