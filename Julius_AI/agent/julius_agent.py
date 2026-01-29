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
    raise ValueError(f"GOOGLE_API_KEY não encontrada no {env_path}")

configure(api_key=api_key)

# 3. MODEL_NAME (Usando o alias estável que apareceu no seu diagnóstico)
MODEL_NAME = "models/gemini-flash-latest"

# Memória temporária para transações não confirmadas
pending_drafts = {}

class JuliusAgent:
    def __init__(self):
        # Inicializa o modelo
        self.model = GenerativeModel(MODEL_NAME)

        self.system_prompt = """
Você é Julius Rock (pai do Chris).
Personalidade: Extremamente pão-duro, sarcástico e engraçado. Odeia gastos.
Sempre que detectar uma transação, use EXATAMENTE o formato abaixo:

VALOR: <número>
CATEGORIA: <Transporte|Alimentação|Lazer|Outros>
TIPO: <Expense|Income>
JULIUS: <sua frase sarcástica sobre o gasto>
"""

    def chat(self, message: str, user_id: str):
        msg_upper = message.upper().strip()

        # --- FLUXO: CONFIRMAR ---
        if msg_upper == "CONFIRMAR":
            if user_id not in pending_drafts:
                return "Julius: Confirmar o quê? Você não me deu nenhum recibo ainda!"
            
            draft_info = pending_drafts[user_id]
            try:
                confirm_transaction(draft_info["draft_id"])
                amount = draft_info["amount"]
                del pending_drafts[user_id]
                return f"Julius: Tá bem, tá bem... Registrei esses R$ {amount:.2f}. Espero que você tenha o cupom fiscal!"
            except Exception as e:
                return f"Julius: O sistema tá fora do ar. Aposto que não pagaram a conta de luz! ({str(e)})"

        # --- FLUXO: CANCELAR ---
        if msg_upper == "CANCELAR":
            if user_id in pending_drafts:
                del pending_drafts[user_id]
                return "Julius: Sábia decisão! Economizou 100% de desconto não comprando nada!"
            return "Julius: Não tem nada para cancelar aqui, Chris."

        # --- FLUXO: GERAR RESPOSTA (GEMINI) ---
        prompt = f"{self.system_prompt}\nUsuário: {message}"
        
        try:
            response = self.model.generate_content(prompt)
            text = response.text
        except google.api_core.exceptions.ResourceExhausted:
            return "Julius: O Google tá me bloqueando! Muita gente falando ao mesmo tempo. Espera um minuto, Chris!"
        except Exception as e:
            return f"Julius: Ih, deu erro na API. Isso deve ser caro... Erro: {str(e)}"

        # --- PARSEAR DADOS ---
        # Regex melhorado para aceitar vírgula ou ponto no valor
        valor_match = re.search(r'VALOR:\s*(\d+(?:[.,]\d+)?)', text)
        categoria_match = re.search(r'CATEGORIA:\s*(Transporte|Alimentação|Lazer|Outros)', text, re.IGNORECASE)
        tipo_match = re.search(r'TIPO:\s*(\w+)', text)
        julius_match = re.search(r'JULIUS:\s*(.+)', text, re.IGNORECASE | re.DOTALL)

        if valor_match and categoria_match:
            amount = float(valor_match.group(1).replace(',', '.'))
            category = categoria_match.group(1).capitalize()
            tx_type = tipo_match.group(1) if tipo_match else "Expense"
            julius_fala = julius_match.group(1).strip() if julius_match else "Vai gastar mesmo?"

            # Enviar para o backend .NET
            # Converter user_id para UUID válido (se for string numérica, gerar um UUID)
            try:
                user_id_guid = str(uuid.UUID(user_id))
            except (ValueError, AttributeError):
                # Se não for UUID válido, gerar um baseado no user_id
                user_id_guid = str(uuid.uuid5(uuid.NAMESPACE_DNS, str(user_id)))
            
            # Type deve ser número: 0=Income, 1=Expense
            type_value = 1 if tx_type == "Expense" else 0
            
            draft_data = {
                "UserId": user_id_guid,
                "Amount": amount,
                "Description": message,
                "Category": category,
                "Type": type_value,
                "Date": "2026-01-29T00:00:00Z", 
                "Confirmed": False
            }

            try:
                result = send_draft_transaction(draft_data)
                pending_drafts[user_id] = {
                    "draft_id": result.get("draftId"),
                    "amount": amount
                }
                return f"{julius_fala}\n\n💰 **R$ {amount:.2f}** | {category}\n\n👉 Digite **CONFIRMAR** ou **CANCELAR**."
            except Exception as e:
                return f"Julius: Tentei salvar mas o servidor soltou um erro: {str(e)}"

        # Caso não seja uma transação financeira, retorna a fala livre do Julius
        return text