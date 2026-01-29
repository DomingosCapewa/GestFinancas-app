import google.generativeai as genai
import os
from dotenv import load_dotenv

load_dotenv()
genai.configure(api_key=os.getenv("GOOGLE_API_KEY"))

try:
    print("Buscando modelos...")
    for m in genai.list_models():
        if 'generateContent' in m.supported_generation_methods:
            print(f"MODELO DISPONÍVEL: {m.name}")
except Exception as e:
    print(f"Erro ao listar: {e}")