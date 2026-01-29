import os
import requests
import urllib3

# Desabilitar warning de SSL
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

DOTNET_API_BASE = os.getenv("DOTNET_API_BASE", "https://localhost:7022/ai/Transaction")

def send_draft_transaction(tx):
    """Cria um rascunho de transação no backend"""
    url = f"{DOTNET_API_BASE}/draft"
    print(f"[DEBUG] Enviando para: {url}")
    print(f"[DEBUG] Payload: {tx}")
    
    try:
        response = requests.post(url, json=tx, verify=False)
        print(f"[DEBUG] Status: {response.status_code}")
        print(f"[DEBUG] Response: {response.text}")
        response.raise_for_status()
        return response.json()
    except requests.exceptions.HTTPError:
        print(f"[ERROR] Detalhes do erro: {response.text}")
        raise

def confirm_transaction(draft_id):
    """Confirma e converte draft em transação definitiva"""
    response = requests.post(f"{DOTNET_API_BASE}/confirm/{draft_id}", verify=False)
    response.raise_for_status()
    return response.json()

def reject_transaction(draft_id):
    """Rejeita e remove o rascunho"""
    response = requests.post(f"{DOTNET_API_BASE}/reject/{draft_id}", verify=False)
    response.raise_for_status()
    return response.json()

def get_user_drafts(user_id):
    """Busca drafts pendentes de um usuário"""
    response = requests.get(f"{DOTNET_API_BASE}/drafts/{user_id}", verify=False)
    response.raise_for_status()
    return response.json()

