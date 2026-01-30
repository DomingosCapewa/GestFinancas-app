import os
import requests
import urllib3

# Desabilitar warning de SSL
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

DOTNET_API_BASE = os.getenv("DOTNET_API_BASE", "https://localhost:7022/ai/Transaction")

def send_draft_transaction(tx, token=None):
    """Cria um rascunho de transação no backend"""
    url = f"{DOTNET_API_BASE}/draft"
    print(f"[DEBUG] Enviando para: {url}")
    print(f"[DEBUG] Payload: {tx}")
    print(f"[DEBUG] Token presente: {'Sim' if token else 'Não'}")
    
    headers = {}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    
    try:
        response = requests.post(url, json=tx, headers=headers, verify=False)
        print(f"[DEBUG] Status: {response.status_code}")
        print(f"[DEBUG] Response: {response.text}")
        response.raise_for_status()
        return response.json()
    except requests.exceptions.HTTPError:
        print(f"[ERROR] Detalhes do erro: {response.text}")
        raise

def confirm_transaction(draft_id, token=None):
    """Confirma e converte draft em transação definitiva"""
    headers = {}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    response = requests.post(f"{DOTNET_API_BASE}/confirm/{draft_id}", headers=headers, verify=False)
    response.raise_for_status()
    return response.json()

def reject_transaction(draft_id, token=None):
    """Rejeita e remove o rascunho"""
    headers = {}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    response = requests.post(f"{DOTNET_API_BASE}/reject/{draft_id}", headers=headers, verify=False)
    response.raise_for_status()
    return response.json()

def get_user_drafts(user_id, token=None):
    """Busca drafts pendentes de um usuário"""
    headers = {}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    response = requests.get(f"{DOTNET_API_BASE}/drafts/{user_id}", headers=headers, verify=False)
    response.raise_for_status()
    return response.json()
