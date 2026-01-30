# GestFinancas - Sistema de Gestão Financeira com IA

> Sistema completo de gestão financeira pessoal com assistente de IA conversacional (Julius Rock), desenvolvido com **Angular 21**, **.NET 9** e **Python/FastAPI** integrado ao **Google Gemini**.

## Visão Geral

O GestFinancas é uma aplicação full-stack que combina:

- **Frontend moderno** em Angular 21 com componentes standalone
- **API REST robusta** em .NET 9 com autenticação JWT e Entity Framework
- **Agente de IA conversacional** (Julius) que interpreta linguagem natural para criar transações financeiras
- **Banco de dados MySQL** para persistência de dados
- **Sistema de draft/confirmação** para transações sugeridas pela IA

## 🏗️ Arquitetura do Sistema

```
┌─────────────────────┐
│   Frontend Angular  │  http://localhost:4200
│   (gestFinancas-    │  
│      com-ia)        │  - Login/Signup/Dashboard
└──────────┬──────────┘  - Gestão de Transações
           │             - Chat com Julius AI
           │
           ├────────────────┐
           │                │
           ▼                ▼
┌──────────────────┐ ┌────────────────────┐
│   .NET 9 API     │ │   Julius AI        │
│ (GestFinancas-   │ │   (FastAPI)        │
│      Api)        │ │                    │
│                  │ │ - Google Gemini    │
│ - Auth JWT       │◄┤ - NLP Processing   │
│ - CRUD Trans.    │ │ - Draft System     │
│ - Email Service  │ └────────────────────┘
└────────┬─────────┘   http://localhost:8000
         │
         ▼
┌─────────────────┐
│   MySQL 8.0+    │
│  (gestfinancas) │
└─────────────────┘
```

## Componentes do Projeto

| Componente | Tecnologia | Descrição | Documentação |
|------------|-----------|-----------|--------------|
| **gestFinancas-com-ia** | Angular 21 | Interface do usuário responsiva | [README](gestFinancas-com-ia/README.md) |
| **GestFinancas-Api** | .NET 9 | API REST com autenticação | [README](GestFinancas-Api/README.md) |
| **Julius_AI** | Python/FastAPI | Agente IA conversacional | [README](Julius_AI/README.MD) |

## Quick Start

### Pré-requisitos

- **Node.js** 18+ e npm
- **Python** 3.10+
- **.NET** 9.0 SDK
- **MySQL** 8.0+
- **Google Gemini API Key** ([Obter aqui](https://makersuite.google.com/app/apikey))

### Instalação Rápida (3 passos)

#### 1️⃣ Backend .NET API

```bash
cd GestFinancas-Api

# Configurar banco de dados em appsettings.json
# Editar ConnectionStrings:DefaultConnection

# Rodar migrações
dotnet ef database update

# Executar
dotnet watch run
```

**Rodando em:** `https://localhost:7022`

#### 2️⃣Julius AI Agent

```bash
cd Julius_AI

# Criar ambiente virtual
python -m venv .venv
# Windows PowerShell
.venv\Scripts\Activate.ps1
# Linux/Mac
source .venv/bin/activate

# Instalar dependências
pip install -r requirements.txt

# Criar .env
cp .env.example .env
# Editar .env e adicionar GOOGLE_API_KEY

# Executar
uvicorn main:app --reload --port 8000
```

**Rodando em:** `http://localhost:8000`

#### 3️⃣ Frontend Angular

```bash
cd gestFinancas-com-ia

# Instalar dependências
npm install

# Executar
npm start
```

**Rodando em:** `http://localhost:4200`

### Primeiro Uso

1. Acesse `http://localhost:4200`
2. Crie uma conta em "Signup"
3. Faça login
4. Vá para o Dashboard ou Chat com Julius
5. Experimente: **"Gastei R$ 50 no Uber hoje"**
6. Julius criará um draft - confirme ou rejeite

## Variáveis de Ambiente

### Julius_AI/.env
```env
GOOGLE_API_KEY=sua_chave_gemini_aqui
DOTNET_API_BASE=https://localhost:7022/ai/Transaction
DB_HOST=localhost
DB_USER=root
DB_PASSWORD=sua_senha
DB_NAME=gestfinancas
ALLOWED_ORIGINS=http://localhost:4200,http://localhost:3000
```

### GestFinancas-Api/appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=GestFinancasDb;uid=root;password=sua_senha;"
  },
  "Jwt": {
    "SecretKey": "sua-chave-secreta-super-longa-com-mais-de-32-caracteres",
    "Issuer": "GestFinancasApi",
    "Audience": "GestFinancasUsers"
  }
}
```

##  Funcionalidades Principais

### Backend (.NET)
- ✅ Autenticação JWT com refresh token
- ✅ CRUD completo de transações
- ✅ Sistema de draft/confirmação para IA
-  Recuperação de senha via email <- em desenvolvimento
- ✅ Validação de dados com DTOs
- ✅ Migrações Entity Framework

### Julius AI (Python)
- ✅ Processamento de linguagem natural com Gemini
- ✅ Detecção inteligente de valores, categorias e tipos
- ✅ Personalidade única (Julius Rock - pai do Chris)
- ✅ Sistema de drafts pendentes
- ✅ Integração com API .NET
- ✅ CORS configurável

### Frontend (Angular)
- ✅ Interface responsiva e moderna
- ✅ Dashboard com visualizações
- ✅ Chat interativo com Julius
- ✅ Gestão de transações
- ✅ Relatórios financeiros
- ✅ Autenticação com interceptors

## 🤖 Como Usar o Julius AI

O Julius entende linguagem natural e responde com humor característico:

```
Usuário: "Gastei R$ 50 no Uber hoje"
Julius: "Uber?! De novo? Aprende a andar de ônibus!"
        [Draft criado - Aguardando confirmação]

Usuário: "CONFIRMAR"
Julius: "Tá bom, registrei essa sangria aí."
        ✅ Transação criada com sucesso
```

### Comandos Reconhecidos
- **Criar transação:** "Gastei X no Y", "Recebi X de Y"
- **Confirmar:** "CONFIRMAR", "SIM", "OK"
- **Cancelar:** "CANCELAR", "NÃO", "DESISTIR"
- **Consultas:** (em desenvolvimento) "Quanto gastei esse mês?"

## Testando a Integração Completa

```bash
# Terminal 1 - Backend
cd GestFinancas-Api
dotnet watch run

# Terminal 2 - Julius AI
cd Julius_AI
.venv\Scripts\Activate.ps1  # ou source .venv/bin/activate
uvicorn main:app --reload

# Terminal 3 - Frontend
cd gestFinancas-com-ia
npm start
```

Acesse `http://localhost:4200` e teste o fluxo completo!

## Troubleshooting

### Julius não responde
- Verifique se está rodando em `http://localhost:8000`
- Confirme `GOOGLE_API_KEY` no arquivo `.env`
- Cheque logs no terminal Python para erros de API
- Teste o endpoint: `curl http://localhost:8000/chat -X POST -H "Content-Type: application/json" -d '{"message":"oi","user_id":"1"}'`

### Erro de CORS
- Frontend deve rodar em `http://localhost:4200`
- Adicione a URL em `ALLOWED_ORIGINS` no `.env` do Julius
- Reinicie o servidor Python após alterar `.env`

### Erro de conexão com banco
- Verifique credenciais em `appsettings.json`
- Confirme que MySQL está rodando: `mysql -u root -p`
- Execute migrations: `dotnet ef database update`
- Verifique se o banco existe: `SHOW DATABASES;`

### Erro de autenticação JWT
- Limpe localStorage do navegador (F12 > Application > Clear)
- Verifique se `SecretKey` tem mais de 32 caracteres
- Confirme que o token está sendo enviado no header

### Frontend não compila
- Delete `node_modules` e `package-lock.json`
- Execute `npm install` novamente
- Verifique versão do Node: `node --version` (precisa 18+)

## 📋 Roadmap / Próximos Passos

- [ ] **Testes:** Unitários (Jest/xUnit), E2E (Cypress/Playwright)
- [ ] **DevOps:** Docker Compose, CI/CD pipeline, Kubernetes
- [ ] **Features:** Exportar relatórios PDF/Excel, Categorias customizáveis
- [ ] **IA:** Histórico de conversas, Análise preditiva de gastos
- [ ] **Segurança:** Rate limiting, 2FA, Audit logs
- [ ] **Mobile:** App React Native ou Progressive Web App
- [ ] **Docs:** Swagger/OpenAPI para todas as APIs

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch: `git checkout -b feature/nova-feature`
3. Commit: `git commit -m 'feat: adiciona nova feature'`
4. Push: `git push origin feature/nova-feature`
5. Abra um Pull Request

### Convenções de Commit
- `feat:` nova funcionalidade
- `fix:` correção de bug
- `docs:` documentação
- `refactor:` refatoração de código
- `test:` adição de testes
- `chore:` tarefas de manutenção

## 📄 Licença

Este projeto é privado e destinado a fins educacionais e demonstração de portfólio.

## 📧 Contato

**Email:** equipe.gest.financas@gmail.com

---

**Desenvolvido com ❤️ usando Angular, .NET, Python e Google Gemini**
