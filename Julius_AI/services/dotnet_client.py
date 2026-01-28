import requests

DOTNET_API = "https://localhost:7022/api/Transaction"

def save_transaction(tx):
    return requests.post(DOTNET_API, json=tx).json()
