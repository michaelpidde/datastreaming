import os
import shutil

try:
    shutil.rmtree('./seed')
except FileNotFoundError:
    pass

try:
    os.remove('./seed_master.bat')
except FileNotFoundError:
    pass

os.makedirs('./seed')

print("Generating seed scripts")

with open("./seed_master.bat", "a", encoding="utf-8") as master:
    master.write("@echo off\n")

with open("generators/customer.py") as f:
    exec(f.read())
with open("generators/product.py") as f:
    exec(f.read())
with open("generators/order.py") as f:
    exec(f.read())

print("Done")