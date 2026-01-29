import os
import requests

DOTNET_API_BASE = os.getenv("DOTNET_API_BASE", "https://localhost:7022/ai/Transaction")


def save_transaction(tx):
    url = f"{DOTNET_API_BASE}"
    return requests.post(url, json=tx).json()
