from google.generativeai import GenerativeModel, configure
import os
import re
import uuid
from pathlib import Path
from dotenv import load_dotenv
import google.api_core.exceptions
from services.dotnet_client import send_draft_transaction, confirm_transaction

# 1. Carregar .env
env_path = Path(__file__).parent.parent / ".env"
load_dotenv(dotenv_path=env_path)

# 2. Configurar API
api_key = os.getenv("GOOGLE_API_KEY")
if not api_key:
    raise ValueError(f"GOOGLE_API_KEY nÃ£o encontrada no {env_path}")

configure(api_key=api_key)

MODEL_NAME = "models/gemini-flash-latest"

# MemÃ³ria temporÃ¡ria para transaÃ§Ãµes nÃ£o confirmadas
pending_drafts = {}

class JuliusAgent:
    def __init__(self):
        # Inicializa o modelo
        self.model = GenerativeModel(MODEL_NAME)

        self.system_prompt = """
VocÃª Ã© Julius Rock (pai do Chris).
Personalidade: Extremamente pÃ£o-duro, sarcÃ¡stico e engraÃ§ado. Odeia gastos.
Sempre que detectar uma transaÃ§Ã£o, use EXATAMENTE o formato abaixo:

VALOR: <nÃºmero>
CATEGORIA: <Transporte|AlimentaÃ§Ã£o|Lazer|Outros>
TIPO: <Expense|Income>
JULIUS: <sua frase sarcÃ¡stica sobre o gasto>
"""

    def chat(self, message: str, user_id: str, token: str = None):
        # --- FLUXO: GERAR RESPOSTA ---
        prompt = f"{self.system_prompt}\nUsuÃ¡rio: {message}"
        
        try:
            response = self.model.generate_content(prompt)
            text = response.text
        except google.api_core.exceptions.ResourceExhausted:
            return "Julius: O Google tÃ¡ me bloqueando! Muita gente falando ao mesmo tempo. Espera um minuto, Chris!"
        except Exception as e:
            return f"Julius: Ih, deu erro na API. Isso deve ser caro... Erro: {str(e)}"

        # --- PARSEAR DADOS ---
        # Regex melhorado para aceitar vírgula ou ponto no valor
        valor_match = re.search(r'VALOR:\s*(\d+(?:[.,]\d+)?)', text)
        categoria_match = re.search(r'CATEGORIA:\s*(Transporte|Alimentação|Lazer|Outros)', text, re.IGNORECASE)
        tipo_match = re.search(r'TIPO:\s*(\w+)', text)
        julius_match = re.search(r'JULIUS:\s*(.+?)(?:\n|$)', text, re.IGNORECASE)

        if valor_match and categoria_match:
            amount = float(valor_match.group(1).replace(',', '.'))
            category = categoria_match.group(1).capitalize()
            tx_type = tipo_match.group(1) if tipo_match else "Expense"
            julius_fala = julius_match.group(1).strip() if julius_match else "Vai gastar mesmo?"

            # Enviar para o backend .NET
            # Converter user_id para int
            try:
                user_id_int = int(user_id)
            except (ValueError, TypeError):
                user_id_int = 1  # Default para testes
            
            # Type deve ser número: 0=Income, 1=Expense
            type_value = 1 if tx_type == "Expense" else 0
            
            draft_data = {
                "UserId": user_id_int,
                "Amount": amount,
                "Description": message,
                "Category": category,
                "Type": type_value,
                "Date": "2026-01-29T00:00:00Z", 
                "Confirmed": False
            }

            try:
                result = send_draft_transaction(draft_data, token)
                pending_drafts[user_id] = {
                    "draft_id": result.get("draftId"),
                    "amount": amount
                }
                # Retornar apenas a mensagem do Julius, sem instruções
                return f" {julius_fala}\n\nR$ {amount:.2f} | {category}\n\nDraft criado! Use os botões para confirmar ou rejeitar."
            except Exception as e:
                return f" Tentei salvar mas o servidor soltou um erro: {str(e)}"
        return text





