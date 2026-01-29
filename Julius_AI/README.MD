# Julius_AI — Agente local

Requisitos de ambiente (mínimos):

- Python 3.10+
- Variáveis de ambiente:
  - `GOOGLE_API_KEY` — chave da API Google Generative AI
  - `DOTNET_API_BASE` (opcional) — URL base da API .NET (padrão: `https://localhost:7022/ai/Transaction`)

Instalação:

```bash
python -m venv .venv
source .venv/bin/activate  # ou .venv\Scripts\Activate.ps1 no Windows PowerShell
pip install -r requirements.txt
```

Rodando o agente (desenvolvimento):

```bash
# A partir da pasta Julius_AI
uvicorn main:app --reload --port 8000
```

Notas rápidas:
- Ajuste `DOTNET_API_BASE` se a API .NET não estiver em `https://localhost:7022`.
- Se usar HTTPS local no .NET, confie no certificado ou use HTTP para testes locais.
- Preencha `requirements.txt` e garanta `GOOGLE_API_KEY` configurada antes de iniciar.