import os
import requests
from models.transaction import TransactionDraft

DOTNET_API_BASE = os.getenv("DOTNET_API_BASE", "https://localhost:7022/ai/Transaction")


def create_transaction_draft(tx: TransactionDraft):
    url = f"{DOTNET_API_BASE}/draft"
    response = requests.post(url, json=tx.dict())
    return response.json()
