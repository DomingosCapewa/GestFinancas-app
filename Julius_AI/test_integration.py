#!/usr/bin/env python
"""Script de teste para integração Julius_AI -> Backend .NET"""
import requests
import json
import sys

# Configuração
JULIUS_URL = "http://localhost:8000/chat"
BACKEND_URL = "https://localhost:7022/ai/Transaction/draft"

def test_agent():
    """Testa o agente Julius_AI"""
    print("=== TESTE DE INTEGRAÇÃO ===\n")
    
    payload = {
        "message": "Gastei R$ 150 no supermercado hoje",
        "user_id": "11111111-1111-1111-1111-111111111111"
    }
    
    print(f"1. Enviando para Julius_AI ({JULIUS_URL}):")
    print(json.dumps(payload, indent=2))
    print()
    
    try:
        response = requests.post(JULIUS_URL, json=payload, timeout=30)
        response.raise_for_status()
        
        print("✓ Resposta do agente:")
        print(json.dumps(response.json(), indent=2))
        print()
        
        return True
    except requests.exceptions.ConnectionError:
        print("✗ Erro: Não foi possível conectar ao agente Julius_AI")
        print("  Certifique-se de que o agente está rodando em http://localhost:8000")
        return False
    except Exception as e:
        print(f"✗ Erro: {e}")
        return False

def test_backend_direct():
    """Testa o backend diretamente"""
    print("\n2. Testando backend diretamente:")
    
    payload = {
        "Id": "00000000-0000-0000-0000-000000000000",
        "UserId": "11111111-1111-1111-1111-111111111111",
        "Amount": 150.0,
        "Description": "Supermercado (teste direto)",
        "Category": "Alimentação",
        "Type": "Expense",
        "Date": "2026-01-28T12:00:00Z",
        "Confirmed": False
    }
    
    try:
        response = requests.post(
            BACKEND_URL, 
            json=payload, 
            timeout=10,
            verify=False  # Ignora SSL para teste local
        )
        response.raise_for_status()
        
        print("✓ Draft criado com sucesso:")
        print(json.dumps(response.json(), indent=2))
        return True
    except requests.exceptions.ConnectionError:
        print("✗ Erro: Não foi possível conectar ao backend .NET")
        print("  Certifique-se de que o backend está rodando em https://localhost:7022")
        return False
    except Exception as e:
        print(f"✗ Erro: {e}")
        return False

if __name__ == "__main__":
    import urllib3
    urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
    
    print("Testando integração Backend + Agente AI\n")
    
    # Testa backend primeiro
    backend_ok = test_backend_direct()
    
    # Testa agente
    agent_ok = test_agent()
    
    print("\n=== RESULTADO ===")
    print(f"Backend: {'✓ OK' if backend_ok else '✗ FALHOU'}")
    print(f"Agente:  {'✓ OK' if agent_ok else '✗ FALHOU'}")
    
    sys.exit(0 if (backend_ok and agent_ok) else 1)
