# GestFinancas-Api

> API REST desenvolvida em **.NET 9** para o sistema de gerenciamento financeiro **GestFinancas**. Fornece autenticação JWT, CRUD de transações, sistema de drafts para IA e recuperação de senha via email.

## Visão Geral

Esta API é o backend central do sistema GestFinancas, responsável por:
- Gerenciar usuários e autenticação
- Armazenar e recuperar transações financeiras
- Processar drafts criados pelo agente Julius AI
- Enviar emails de recuperação de senha
- Validar e persistir dados no MySQL

## Tecnologias

- **.NET 9.0** - Framework web moderno
- **Entity Framework Core** - ORM para MySQL
- **MySQL 8.0+** - Banco de dados relacional
- **JWT** - Autenticação stateless
- **Swagger/OpenAPI** - Documentação de API
- **SMTP** - Envio de emails

## ⚙️ Configuração e Instalação

### Pré-requisitos

- **.NET 9.0 SDK** ([Download](https://dotnet.microsoft.com/download))
- **MySQL 8.0+** rodando localmente ou remoto
- **Ferramenta de teste** (Postman, Insomnia, Thunder Client)

### Instalação

1. **Clone o repositório:**
   ```bash
   git clone https://git.gft.com/dscw/GestFinancas-app.git
   cd GestFinancas-Api
   ```

2. **Configure o banco de dados:**
   
   Edite `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "server=localhost;port=3306;database=GestFinancasDb;uid=root;password=sua_senha;"
     },
     "Jwt": {
       "SecretKey": "sua-chave-secreta-super-longa-com-mais-de-32-caracteres-para-seguranca",
       "Issuer": "GestFinancasApi",
       "Audience": "GestFinancasUsers"
     }
   }
   ```

3. **Restaure as dependências:**
   ```bash
   dotnet restore
   ```

4. **Execute as migrações:**
   ```bash
   dotnet ef database update
   ```
   
   Isso criará automaticamente o banco de dados e as tabelas necessárias.

5. **Inicie o servidor:**
   ```bash
   dotnet run
   # ou para hot-reload em desenvolvimento
   dotnet watch run
   ```

6. **Acesse a API:**
   - HTTPS: `https://localhost:7022`
   - HTTP: `http://localhost:5282`
   - Swagger UI: `https://localhost:7022/swagger`

## 📁 Estrutura do Projeto

```
GestFinancas-Api/
├── Controllers/          # Endpoints da API
│   ├── TransactionController.cs    # CRUD de transações + drafts
│   ├── UsuarioController.cs        # Auth e gerenciamento de usuários
│   ├── EmailController.cs          # Recuperação de senha
│   ├── DebugTokenController.cs     # Debug JWT (dev only)
│   └── DatabaseFixController.cs    # Utilitários de banco (dev only)
├── Models/               # Entidades do banco de dados
│   ├── Usuario.cs                  # Modelo de usuário
│   ├── Transaction.cs              # Transação confirmada
│   ├── DraftTransaction.cs         # Rascunho de transação (IA)
│   ├── UserToken.cs                # Tokens de recuperação de senha
│   ├── AuditLogs.cs                # Logs de auditoria
│   └── AppDbContext.cs             # Contexto EF Core
├── Dtos/                 # Data Transfer Objects
│   ├── CreateTransactionDto.cs
│   ├── LoginDto.cs
│   └── RedefinirSenhaTokenDto.cs
├── Data/                 # Repositórios e interfaces
│   ├── IUsuarioRepository.cs
│   ├── UsuarioRepository.cs
│   ├── ITransactionRepository.cs
│   └── TransactionRepository.cs
├── Services/             # Lógica de negócios
│   ├── TokenService.cs             # Geração de JWT
│   └── UsuarioService.cs
├── Identity/             # Autenticação
│   ├── IAuthenticate.cs
│   └── Authenticate.cs
├── Helper/               # Utilitários
│   └── emailHelper/
│       └── EnviarEmail.cs
├── Migrations/           # Migrações EF Core
├── Configurations/
│   └── Program.cs        # Configuração da aplicação
├── appsettings.json      # Configurações
└── GestFinancas-Api.csproj
```

## 🔌 Endpoints da API

### 🔐 Autenticação (`/api/Usuario`)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| `POST` | `/login` | Faz login e retorna JWT | ❌ |
| `POST` | `/cadastrar-usuario` | Registra novo usuário | ❌ |
| `GET` | `/` | Lista todos usuários | ✅ |
| `PUT` | `/` | Atualiza dados do usuário | ✅ |
| `POST` | `/confirmar-reset-senha` | Reset de senha via token | ❌ |

### 💰 Transações (`/ai/Transaction` ou `/api/Transaction`)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| `GET` | `/` | Lista transações do usuário | ✅ |
| `POST` | `/` | Cria nova transação | ✅ |
| `POST` | `/draft` | Cria draft de transação (IA) | ❌ |
| `GET` | `/drafts/{userId}` | Lista drafts pendentes | ❌ |
| `POST` | `/confirm/{id}` | Confirma um draft | ✅ |
| `POST` | `/reject/{id}` | Rejeita e remove draft | ✅ |

### ✉️ Email (`/api/Email`)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| `POST` | `/email-recuperacao-senha` | Envia email de recuperação | ❌ |
| `POST` | `/confirmar-cadastro` | Envia email de confirmação | ❌ |

## 🔒 Autenticação JWT

A API usa JWT (JSON Web Tokens) para autenticação. 

### Fluxo de Autenticação

1. **Login:** `POST /api/Usuario/login`
   ```json
   {
     "email": "usuario@email.com",
     "senha": "senha123"
   }
   ```

2. **Resposta com Token:**
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "usuario": {
       "id": 1,
       "nome": "João Silva",
       "email": "usuario@email.com"
     }
   }
   ```

3. **Usar Token nas Requisições:**
   ```
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

### Claims do Token
- `id` - ID do usuário
- `email` - Email do usuário
- `http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier` - Nome

## Modelos de Dados

### Usuario
```csharp
{
  "id": 1,
  "nome": "João Silva",
  "email": "joao@email.com",
  "senha": "hash_bcrypt",
  "dataCriacao": "2026-01-30T00:00:00Z"
}
```

### Transaction (Confirmada)
```csharp
{
  "id": "guid",
  "userId": 1,
  "amount": 150.50,
  "description": "Compras no supermercado",
  "category": "Alimentação",
  "type": "Expense",  // ou "Income"
  "date": "2026-01-30T00:00:00Z",
  "source": "AI",     // ou "Manual"
  "createdAt": "2026-01-30T12:30:00Z"
}
```

### DraftTransaction (Pendente)
```csharp
{
  "id": "guid",
  "userId": 1,
  "amount": 50.00,
  "description": "Uber para o trabalho",
  "category": "Transporte",
  "type": "Expense",
  "date": "2026-01-30T00:00:00Z",
  "confirmed": false
}
```
  - `POST /api/transactions`: Criar uma nova transação.
  - `PUT /api/transactions/{id}`: Atualizar uma transação.

## 🧪 Exemplos de Uso

### Criar um usuário
```bash
curl -X POST https://localhost:7022/api/Usuario/cadastrar-usuario \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "email": "joao@email.com",
    "senha": "senha123"
  }'
```

### Fazer login
```bash
curl -X POST https://localhost:7022/api/Usuario/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@email.com",
    "senha": "senha123"
  }'
```

### Criar transação (com token)
```bash
curl -X POST https://localhost:7022/api/Transaction \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "amount": 150.50,
    "description": "Compras no supermercado",
    "category": "Alimentação",
    "type": "Expense"
  }'
```

### Criar draft (usado pela IA - sem auth)
```bash
curl -X POST https://localhost:7022/ai/Transaction/draft \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 1,
    "amount": 50.00,
    "description": "Uber para o trabalho",
    "category": "Transporte",
    "type": "Expense"
  }'
```

### Confirmar draft
```bash
curl -X POST https://localhost:7022/ai/Transaction/confirm/GUID_DO_DRAFT \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

## Migrações do Banco de Dados

### Criar nova migração
```bash
dotnet ef migrations add NomeDaMigracao
```

### Aplicar migrações
```bash
dotnet ef database update
```

### Reverter última migração
```bash
dotnet ef database update MigracaoAnterior
```

### Remover última migração
```bash
dotnet ef migrations remove
```

## 🐛 Debug e Desenvolvimento

### Rodar com hot-reload
```bash
dotnet watch run
```

### Ver logs detalhados
Edite `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### Acessar Swagger UI
Navegue para: `https://localhost:7022/swagger`

## 🔐 Segurança

### Práticas Implementadas
- ✅ Senhas hashadas com BCrypt
- ✅ JWT com expiração configurável
- ✅ CORS configurado
- ✅ HTTPS habilitado por padrão
- ✅ Validação de DTOs
- ✅ Autenticação em endpoints sensíveis

### Melhorias em Desenvolvimento
- [ ] Rate limiting
- [ ] Refresh tokens
- [ ] 2FA (Two-Factor Authentication)
- [ ] Logging de auditoria expandido
- [ ] Criptografia de dados sensíveis no banco

## 🧩 Integração com Julius AI

A API possui endpoints especiais para integração com o agente Julius AI:

1. **Julius cria draft:** `POST /ai/Transaction/draft` (sem auth)
2. **Usuário confirma via frontend:** `POST /ai/Transaction/confirm/{id}` (com auth)
3. **Draft vira Transaction:** Movido para tabela `Transactions` com `Source = AI`

## 🛠️ Troubleshooting

### Erro: "Unable to connect to MySQL"
```bash
# Verificar se MySQL está rodando
mysql -u root -p

# Verificar credenciais em appsettings.json
# Verificar firewall e porta 3306
```

### Erro: "Pending migrations"
```bash
dotnet ef database update
```

### Erro: "Invalid JWT token"
- Verificar se `SecretKey` tem 32+ caracteres
- Confirmar que o token não expirou
- Verificar formato do header: `Authorization: Bearer TOKEN`

### Erro ao enviar email
- Configurar SMTP em `appsettings.json`
- Verificar credenciais do servidor de email
- Checar firewall para porta 587/465

## Recursos Adicionais

- [Documentação .NET 9](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [JWT Authentication](https://jwt.io/)
- [MySQL Connector](https://dev.mysql.com/doc/connector-net/en/)

## 👥 Contribuindo

Consulte o [README principal](../README.md) para guidelines de contribuição.

## 📄 Licença

Projeto educacional - Ver [README principal](../README.md) para mais informações.

