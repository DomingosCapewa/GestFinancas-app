# GestFinancas - Sistema de Gestão Financeira com IA

Sistema completo de gestão financeira com assistente IA (Julius) baseado em Google Gemini, integrado com frontend Angular e backend .NET.

## Pré-requisitos

- **Node.js** 18+ e npm
- **Python** 3.9+
- **.NET** 9.0 SDK
- **MySQL** 8.0+
- **Google Gemini API Key** ([Obter aqui](https://makersuite.google.com/app/apikey))

## 🏗️ Estrutura do Projeto

```
GestFinancas-app/
├── gestFinancas-com-ia/     # Frontend Angular
├── GestFinancas-Api/         # Backend .NET API
└── Julius_AI/                # Agente IA Python (FastAPI)
```

## Configuração

### 1. Backend .NET API

```bash
cd GestFinancas-Api

# Configurar banco de dados em appsettings.json
# ConnectionStrings:DefaultConnection

# Rodar migrações
dotnet ef database update

# Executar
dotnet watch run
```

**URL padrão:** `https://localhost:7022` ou `http://localhost:5282`

### 2. Agente Julius AI (Python)

```bash
cd Julius_AI

# Instalar dependências
pip install -r requirements.txt

# Criar arquivo .env baseado em .env.example
cp .env.example .env

# Editar .env e adicionar:
# GOOGLE_API_KEY=sua_chave_aqui
# DOTNET_API_BASE=https://localhost:7022/ai/Transaction
# DB_HOST=localhost
# DB_USER=root
# DB_PASSWORD=sua_senha
# DB_NAME=gestfinancas

# Executar servidor
uvicorn main:app --reload --port 8000
```

**URL padrão:** `http://localhost:8000`

### 3. Frontend Angular

```bash
cd gestFinancas-com-ia

# Instalar dependências
npm install

# Criar arquivo .env baseado em .env.example (opcional)
cp .env.example .env

# Executar
npm start
# ou
ng serve
```

**URL padrão:** `http://localhost:4200`

## 🔐 Variáveis de Ambiente

### Julius_AI/.env
```env
GOOGLE_API_KEY=sua_chave_gemini
DOTNET_API_BASE=https://localhost:7022/ai/Transaction
DB_HOST=localhost
DB_USER=root
DB_PASSWORD=senha
DB_NAME=gestfinancas
ALLOWED_ORIGINS=http://localhost:4200,http://localhost:3000
```

### GestFinancas-Api/appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=gestfinancas;User=root;Password=senha;"
  },
  "Jwt": {
    "SecretKey": "sua_chave_secreta_jwt_aqui"
  }
}
```

## Funcionalidades

### Implementadas

- 🔐 Autenticação JWT
- 💰 CRUD de transações financeiras
- 🤖 Assistente IA Julius (conversacional)
- ✉️ Envio de emails
- 📊 Dashboard com gráficos
- 💾 Sistema de drafts/aprovação de transações
- 🔄 Integração completa Frontend-Backend-IA

### Julius AI - Comandos

O agente Julius entende comandos em linguagem natural:

- **Registrar gastos:** "Gastei R$ 50 no Uber hoje"
- **Confirmar transação:** "CONFIRMAR"
- **Cancelar transação:** "CANCELAR"
- **Consultas:** "Quanto gastei esse mês?"

## Testando a Integração

1. Inicie os 3 servidores (ordem recomendada):
   - Backend .NET: `dotnet watch run`
   - Julius AI: `uvicorn main:app --reload`
   - Frontend: `ng serve`

2. Acesse `http://localhost:4200`

3. Navegue até a página do Julius AI

4. Digite: "Gastei R$ 100 no supermercado"

5. Julius criará um draft - confirme ou rejeite através dos botões


## 📝 Próximos Passos (Backlog)

- [ ] Testes unitários (Backend + Frontend)
- [ ] Testes E2E com Cypress
- [ ] Rate limiting no Python API
- [ ] Logging estruturado
- [ ] Docker compose para ambiente completo
- [ ] CI/CD pipeline
- [ ] Documentação Swagger para Julius API
- [ ] Histórico de conversas persistido
- [ ] Melhorar predições com sklearn

## Troubleshooting

### Julius não responde
- Verifique se o servidor está rodando em `http://localhost:8000`
- Confirme se a `GOOGLE_API_KEY` está configurada
- Veja logs no terminal do Python para erros de API

### Erro de CORS
- Certifique-se que o frontend está rodando em `http://localhost:4200`
- Adicione a URL em `ALLOWED_ORIGINS` no .env do Julius

### Erro de conexão com banco
- Verifique credenciais em `appsettings.json`
- Confirme que o MySQL está rodando
- Execute as migrations: `dotnet ef database update`

## Contribuindo

1. Fork o projeto
2. Crie uma branch: `git checkout -b feature/nova-feature`
3. Commit suas mudanças: `git commit -m 'Add nova feature'`
4. Push para a branch: `git push origin feature/nova-feature`
5. Abra um Pull Request

## Licença

Este projeto é privado e destinado a fins educacionais.

## Contato
Email: equipe.gest.financas@gmail.com
