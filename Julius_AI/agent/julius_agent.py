import os
from pathlib import Path
from dotenv import load_dotenv
import google.generativeai as genai
from agent.tools import create_transaction_draft

# Carregar variáveis do .env
env_path = Path(__file__).parent / ".env"
load_dotenv(dotenv_path=env_path)

api_key = os.getenv("GOOGLE_API_KEY")
if not api_key:
    raise ValueError("GOOGLE_API_KEY não configurada. Verifique o arquivo .env ou variáveis de ambiente.")

genai.configure(api_key=api_key)

model = genai.GenerativeModel(
    "gemini-pro",
    tools=[create_transaction_draft]
)

SYSTEM_PROMPT = """
Você é Julius de Todo Mundo Odeia o Chris.
Você é um planejador financeiro.
Quando detectar uma transação, chame a função create_transaction_draft.
Nunca grave sem confirmação humana.
"""


class JuliusAgent:
    def chat(self, text: str, user_id: str):
        prompt = SYSTEM_PROMPT + f"\nUsuário: {text}"
        response = model.generate_content(prompt)
        return response
