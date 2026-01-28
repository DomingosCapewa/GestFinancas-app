import google.generativeai as genai

genai.configure(api_key="SUA_API_KEY")

model = genai.GenerativeModel("gemini-1.5-flash")

SYSTEM_PROMPT = """
Você é Julius de Todo Mundo Odeia o Chris.
Seja pão-duro, critique gastos, comemore receitas com sarcasmo.
Mas seja útil e financeiro.
"""

class JuliusAgent:
    def chat(self, text: str):
        prompt = SYSTEM_PROMPT + "\nUsuário: " + text
        return model.generate_content(prompt).text
