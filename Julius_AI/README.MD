# Julius AI - Agente Conversacional de Finanças

> Agente de IA conversacional desenvolvido em **Python/FastAPI** que usa **Google Gemini** para processar linguagem natural e criar transações financeiras. Integrado com a API .NET do GestFinancas.

## 📋 Visão Geral

Julius Rock (inspirado no pai do Chris da série "Todo Mundo Odeia o Chris") é um assistente financeiro com personalidade única:
- **Sarcástico e engraçado** ao comentar gastos
- **Pão-duro por natureza** - sempre critica despesas
- **Inteligente** - detecta valores, categorias e tipos de transação automaticamente
- **Conversacional** - entende linguagem natural em português

## Funcionalidades

-  **Processamento de Linguagem Natural** via Google Gemini
-  **Detecção automática** de valores, categorias e tipos
-  **Sistema de confirmação** (draft → confirm/cancel)
-  **Integração com API .NET** para persistência
-  **Personalidade única** com respostas humorísticas
-  **API REST FastAPI** para comunicação com frontend
-  **CORS configurável** para múltiplas origens

##  Instalação e Configuração

### Pré-requisitos

- **Python 3.10+** ([Download](https://python.org))
- **Google Gemini API Key** ([Obter aqui](https://makersuite.google.com/app/apikey))
- **API .NET** rodando em `https://localhost:7022`

### Instalação

1. **Navegue até o diretório:**
   ```bash
   cd Julius_AI
   ```

2. **Crie ambiente virtual:**
   ```bash
   # Windows
   python -m venv .venv
   .venv\Scripts\Activate.ps1
   
   # Linux/Mac
   python3 -m venv .venv
   source .venv/bin/activate
   ```

3. **Instale dependências:**
   ```bash
   pip install -r requirements.txt
   ```

4. **Configure variáveis de ambiente:**
   
   Crie arquivo `.env` na raiz do projeto:
   ```env
   GOOGLE_API_KEY=sua_chave_gemini_aqui
   DOTNET_API_BASE=https://localhost:7022/ai/Transaction
   DB_HOST=localhost
   DB_USER=root
   DB_PASSWORD=sua_senha
   DB_NAME=gestfinancas
   ALLOWED_ORIGINS=http://localhost:4200,http://localhost:3000
   ```

5. **Inicie o servidor:**
   ```bash
   uvicorn main:app --reload --port 8000
   ```

6. **Verifique se está rodando:**
   
   Acesse: `http://localhost:8000/docs` (Swagger UI automático)

## 📁 Estrutura do Projeto

```
Julius_AI/
├── main.py                    # Entrada da aplicação FastAPI
├── requirements.txt           # Dependências Python
├── .env                       # Variáveis de ambiente (criar)
├── .env.example               # Exemplo de configuração
├── agent/
│   ├── __init__.py
│   ├── julius_agent.py        # Lógica principal do agente
│   ├── memory.py              # Sistema de memória (draft)
│   ├── parser.py              # Parse de resposta do Gemini
│   ├── predictor.py           # Predições (futuro)
│   ├── tools.py               # Ferramentas auxiliares
│   └── transaction.py         # Tipos de transação
├── models/
│   └── transaction.py         # Modelos Pydantic
└── services/
    └── dotnet_client.py       # Cliente HTTP para API .NET
```

## 🔌 API Endpoints

### `POST /chat`

Envia mensagem para Julius e recebe resposta.

**Request:**
```json
{
  "message": "Gastei R$ 50 no Uber hoje",
  "user_id": "1",
  "token": "opcional_jwt_token"
}
```

**Response:**
```json
{
  "response": "Uber?! De novo? Aprende a andar de ônibus!\n\n Draft criado! Digite CONFIRMAR para salvar ou CANCELAR para desistir."
}
```

##  Como Funciona

### Fluxo de Conversa

```
┌─────────────────────────────────────────────────┐
│ Usuário: "Gastei R$ 50 no Uber hoje"           │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│ Julius Agent processa com Google Gemini        │
│ - Extrai: valor=50, categoria=Transporte       │
│ - Tipo: Expense                                 │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│ Cria Draft via API .NET                         │
│ POST /ai/Transaction/draft                      │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│ Salva em memória local (pending_drafts)        │
│ Aguarda "CONFIRMAR" ou "CANCELAR"               │
└─────────────────────────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│ Usuário: "CONFIRMAR"                            │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│ Julius confirma draft via API .NET              │
│ POST /ai/Transaction/confirm/{id}               │
│ (com token JWT do usuário)                      │
└─────────────────────────────────────────────────┘
```

### Categorias Reconhecidas

- **Transporte** - Uber, táxi, gasolina, ônibus
- **Alimentação** - Restaurante, supermercado, delivery
- **Lazer** - Cinema, shows, viagens
- **Outros** - Qualquer outra coisa

### Tipos de Transação

- **Expense** (Despesa) - Gasto
- **Income** (Receita) - Ganho

## 🧩 Integração com API .NET

O Julius se comunica com a API .NET através do cliente HTTP em `services/dotnet_client.py`:

### Criar Draft
```python
POST {DOTNET_API_BASE}/draft
{
  "userId": 1,
  "amount": 50.0,
  "description": "Uber",
  "category": "Transporte",
  "type": "Expense"
}
```

### Confirmar Draft
```python
POST {DOTNET_API_BASE}/confirm/{draft_id}
Headers: {
  "Authorization": "Bearer {token}"
}
```

## 🛠️ Desenvolvimento

### Rodar testes
```bash
# Teste rápido do modelo
python teste_modelo.py
```

### Debug de Gemini
```bash
# Ver resposta bruta do Gemini
# Adicione prints em julius_agent.py linha ~50
print(f"Resposta Gemini: {raw_response}")
```

### Adicionar nova categoria
Edite `julius_agent.py`:
```python
self.system_prompt = """
CATEGORIA: <Transporte|Alimentação|Lazer|Saúde|Outros>
"""
```

## 🔐 Segurança

### Práticas Implementadas
- ✅ CORS configurável via `.env`
- ✅ Validação de dados com Pydantic
- ✅ Token JWT opcional para operações sensíveis
- ✅ HTTPS na comunicação com API .NET

### Melhorias em Desenvolvimento
- [ ] Rate limiting por usuário
- [ ] Validação de token JWT no Julius
- [ ] Sanitização de inputs
- [ ] Logging estruturado
- [ ] Monitoramento de uso da API Gemini

## 🧪 Exemplos de Uso

### Testar localmente
```bash
curl -X POST http://localhost:8000/chat \
  -H "Content-Type: application/json" \
  -d '{
    "message": "Gastei 100 reais no supermercado",
    "user_id": "1"
  }'
```

### Resposta esperada
```json
{
  "response": "Supermercado de novo?! Tá comendo ouro?!\n\n Draft criado! Digite CONFIRMAR para salvar."
}
```

### Confirmar
```bash
curl -X POST http://localhost:8000/chat \
  -H "Content-Type: application/json" \
  -d '{
    "message": "CONFIRMAR",
    "user_id": "1",
    "token": "seu_token_jwt"
  }'
```

## 🐛 Troubleshooting

### Erro: "GOOGLE_API_KEY não encontrada"
- Verifique se `.env` existe na raiz do projeto
- Confirme que a chave está correta: [Testar aqui](https://makersuite.google.com/app/apikey)
- Reinicie o servidor após adicionar

### Erro: "Connection refused" (API .NET)
- Verifique se a API .NET está rodando
- Confirme URL em `DOTNET_API_BASE` no `.env`
- Se usar HTTPS local, confie no certificado:
  ```bash
  dotnet dev-certs https --trust
  ```

### Julius não detecta valores
- Frases devem conter números explícitos: "R$ 50" ou "50 reais"
- Evite abreviações complexas
- Exemplo bom: "Gastei 50 no Uber"
- Exemplo ruim: "Gastei no Uber" (sem valor)

### CORS error no frontend
- Adicione URL do frontend em `ALLOWED_ORIGINS` no `.env`
- Formato: `http://localhost:4200,http://localhost:3000`
- Reinicie o servidor Python

##  Dependências Principais

```txt
fastapi==0.104.1          # Framework web
uvicorn==0.24.0           # Servidor ASGI
google-generativeai==0.3.1 # Cliente Gemini
pydantic==2.5.0           # Validação de dados
python-dotenv==1.0.0      # Variáveis de ambiente
requests==2.31.0          # Cliente HTTP
```

##  Roadmap

- [ ] Persistir histórico de conversas no banco
- [ ] Análise preditiva de gastos (sklearn)
- [ ] Consultas: "Quanto gastei esse mês?"
- [ ] Suporte a múltiplos idiomas
- [ ] Categorização automática melhorada
- [ ] Webhooks para notificações
- [ ] Dashboard de uso da API Gemini

## 👥 Contribuindo

Consulte o [README principal](../README.md) para guidelines de contribuição.

## 📄 Licença

Projeto educacional - Ver [README principal](../README.md) para mais informações.
