# gestFinancas-com-ia

> Frontend moderno desenvolvido em **Angular 19** com componentes standalone para o sistema de gestão financeira GestFinancas. Interface responsiva com dashboard, gestão de transações e chat com o agente Julius AI.

## 📋 Visão Geral

Este é o frontend do sistema GestFinancas, que oferece:
- **Interface moderna e responsiva** para gestão financeira
- **Dashboard interativo** com visualizações de dados
- **Chat com Julius AI** para criar transações via linguagem natural
- **Autenticação JWT** com interceptors
- **Gestão completa de transações** (CRUD)
- **Relatórios financeiros** personalizados

## 🚀 Tecnologias

- **Angular 19** - Framework web com componentes standalone
- **TypeScript 5+** - Linguagem tipada
- **RxJS** - Programação reativa
- **Signals** - Gerenciamento de estado moderno
- **HttpClient** - Comunicação com APIs
- **Standalone Components** - Arquitetura moderna sem NgModules

## ⚙️ Configuração e Instalação

### Pré-requisitos

- **Node.js 18+** ([Download](https://nodejs.org))
- **npm** (incluído com Node.js)
- **Angular CLI** (opcional, mas recomendado)

### Instalação

1. **Navegue até o diretório:**
   ```bash
   cd gestFinancas-com-ia
   ```

2. **Instale as dependências:**
   ```bash
   npm install
   ```

3. **Configure ambientes (opcional):**
   
   Edite `src/environments/environment.ts` para desenvolvimento:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'https://localhost:7022/api',
     juliusApiUrl: 'http://localhost:8000'
   };
   ```

4. **Inicie o servidor de desenvolvimento:**
   ```bash
   npm start
   # ou
   ng serve
   ```

5. **Acesse a aplicação:**
   
   Navegue para: `http://localhost:4200`

## 📁 Estrutura do Projeto

```
gestFinancas-com-ia/
├── src/
│   ├── app.component.ts          # Componente raiz
│   ├── app.routes.ts             # Configuração de rotas
│   ├── components/
│   │   └── ai-agent/             # Componente do chat Julius
│   ├── environments/
│   │   ├── environment.ts        # Config desenvolvimento
│   │   └── environment.prod.ts   # Config produção
│   ├── interceptors/
│   │   └── auth.interceptor.ts   # Interceptor JWT (provável)
│   ├── models/
│   │   └── *.model.ts            # Interfaces TypeScript
│   ├── pages/
│   │   ├── login/                # Página de login
│   │   ├── signup/               # Página de cadastro
│   │   ├── dashboard/            # Dashboard principal
│   │   ├── transactions/         # Gestão de transações
│   │   ├── reports/              # Relatórios financeiros
│   │   └── forgot-password/      # Recuperação de senha
│   ├── services/
│   │   ├── auth/                 # Serviço de autenticação
│   │   ├── transaction.service.ts
│   │   ├── julius-agent.service.ts
│   │   ├── gemini.service.ts
│   │   └── enviarEmail.service.ts
│   └── utils/                    # Utilitários e helpers
├── angular.json                  # Configuração do Angular
├── tsconfig.json                 # Configuração TypeScript
├── package.json                  # Dependências npm
└── README.md                     # Este arquivo
```

## 🛣️ Rotas da Aplicação

| Rota | Componente | Descrição | Auth |
|------|------------|-----------|------|
| `/login` | LoginComponent | Login de usuário | ❌ |
| `/signup` | SignupComponent | Cadastro de novo usuário | ❌ |
| `/forgot-password` | ForgotPasswordComponent | Recuperação de senha | ❌ |
| `/dashboard` | DashboardComponent | Painel principal | ✅ |
| `/transactions` | TransactionsComponent | Lista e CRUD de transações | ✅ |
| `/reports` | ReportsComponent | Relatórios financeiros | ✅ |
| `/` | Redirect → `/login` | Redirecionamento padrão | - |

## 🔌 Serviços Principais

### AuthService
Gerencia autenticação e autorização:
```typescript
// Login
login(email: string, password: string): Observable<AuthResponse>

// Cadastro
signup(user: User): Observable<void>

// Verificar autenticação
isAuthenticated(): boolean

// Logout
logout(): void

// Obter token
getToken(): string | null
```

### TransactionService
Gerencia transações financeiras:
```typescript
// Listar todas
getTransactions(): Observable<Transaction[]>

// Criar nova
createTransaction(transaction: CreateTransactionDto): Observable<Transaction>

// Atualizar
updateTransaction(id: string, transaction: Transaction): Observable<Transaction>

// Deletar
deleteTransaction(id: string): Observable<void>

// Obter drafts
getDrafts(userId: number): Observable<DraftTransaction[]>

// Confirmar draft
confirmDraft(id: string): Observable<Transaction>

// Rejeitar draft
rejectDraft(id: string): Observable<void>
```

### JuliusAgentService
Comunicação com o agente Julius AI:
```typescript
// Enviar mensagem
sendMessage(message: string, userId: string, token?: string): Observable<ChatResponse>

// Stream de mensagens (se implementado)
streamChat(message: string): Observable<string>
```

## 🎨 Componentes Principais

### Dashboard
- Exibe resumo financeiro
- Gráficos de receitas vs despesas
- Transações recentes
- Cards com totalizadores

### Transactions
- Lista de todas as transações
- Filtros por data, categoria, tipo
- Adicionar/editar/excluir transações
- Visualização em tabela ou cards

### AI Agent (Julius Chat)
- Interface de chat conversacional
- Envio de mensagens para Julius
- Exibição de drafts pendentes
- Botões para confirmar/rejeitar drafts

## 🔒 Autenticação e Segurança

### Fluxo de Autenticação

1. **Login:** Usuário envia credenciais
2. **Backend retorna JWT:** Token armazenado no localStorage
3. **Interceptor adiciona token:** Todas as requisições incluem `Authorization: Bearer TOKEN`
4. **Guard protege rotas:** Redireciona para login se não autenticado

### Auth Interceptor (exemplo)
```typescript
intercept(req: HttpRequest<any>, next: HttpHandler) {
  const token = this.authService.getToken();
  
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  
  return next.handle(req);
}
```

## 🧪 Desenvolvimento

### Comandos Disponíveis

```bash
# Servidor de desenvolvimento
npm start
ng serve

# Build de produção
npm run build
ng build --configuration production

# Testes unitários
npm test
ng test

# Testes E2E (se configurado)
npm run e2e

# Linting
ng lint

# Análise de bundle
npm run build -- --stats-json
npx webpack-bundle-analyzer dist/stats.json
```

### Hot Reload
O servidor de desenvolvimento tem hot-reload automático. Mudanças no código são refletidas instantaneamente no navegador.

### Proxy para APIs (opcional)
Crie `proxy.conf.json` para evitar CORS em dev:
```json
{
  "/api": {
    "target": "https://localhost:7022",
    "secure": false,
    "changeOrigin": true
  },
  "/chat": {
    "target": "http://localhost:8000",
    "secure": false,
    "changeOrigin": true
  }
}
```

Rode com:
```bash
ng serve --proxy-config proxy.conf.json
```

## 🎨 Estilização

### CSS/SCSS
- Estilos globais em `src/styles.css` ou `src/styles.scss`
- Estilos de componente em arquivos `.component.css`
- Possível uso de TailwindCSS ou Material Design

### Temas (se aplicável)
- Tema claro/escuro
- Customização de cores
- Responsividade mobile-first

## 📱 Responsividade

A aplicação é totalmente responsiva:
- **Desktop** - Layout completo com sidebar
- **Tablet** - Layout adaptado
- **Mobile** - Menu hamburguer, cards empilhados

## 🧩 Integração com Backend

### API .NET (`environment.apiUrl`)
```typescript
// Base URL
apiUrl: 'https://localhost:7022/api'

// Endpoints usados
GET    /api/Usuario          # Listar usuários
POST   /api/Usuario/login    # Login
POST   /api/Usuario/cadastrar-usuario  # Signup
GET    /api/Transaction      # Listar transações
POST   /api/Transaction      # Criar transação
POST   /api/Transaction/confirm/{id}  # Confirmar draft
POST   /api/Transaction/reject/{id}   # Rejeitar draft
```

### Julius AI (`environment.juliusApiUrl`)
```typescript
// Base URL
juliusApiUrl: 'http://localhost:8000'

// Endpoints usados
POST   /chat  # Enviar mensagem para Julius
```

## 🐛 Troubleshooting

### Erro: "Cannot GET /"
- Verifique se está acessando `http://localhost:4200` (não `/`)
- Limpe cache do navegador
- Reinicie o servidor: `ng serve`

### Erro: CORS ao chamar API
- Verifique se backend tem CORS configurado
- Use proxy config (veja seção acima)
- Confirme URLs em `environment.ts`

### Erro: "Token expired" ou 401
- Faça logout e login novamente
- Limpe localStorage: F12 → Application → Clear
- Verifique se token JWT está válido

### Build falha com erros TypeScript
- Verifique tipos em `models/`
- Execute `npm install` novamente
- Atualize Angular CLI: `npm i -g @angular/cli@latest`

### App não compila após atualização
```bash
# Limpar cache
rm -rf node_modules package-lock.json
npm install

# Atualizar Angular
ng update @angular/core @angular/cli
```

## 📦 Build para Produção

```bash
# Build otimizado
npm run build
# ou
ng build --configuration production

# Output em dist/
cd dist/gestFinancas-com-ia

# Servir com nginx, apache ou qualquer servidor static
```

### Configuração de Servidor Web

**Nginx exemplo:**
```nginx
server {
  listen 80;
  server_name gestfinancas.com;
  root /var/www/gestfinancas;
  
  location / {
    try_files $uri $uri/ /index.html;
  }
  
  location /api {
    proxy_pass https://api.gestfinancas.com;
  }
}
```

## 🔮 Features Futuras

- [ ] PWA (Progressive Web App)
- [ ] Notificações push
- [ ] Modo offline
- [ ] Gráficos avançados (Chart.js, D3.js)
- [ ] Exportação de relatórios PDF
- [ ] Tema dark mode persistente
- [ ] Multi-idioma (i18n)
- [ ] Animações e transições

## 📚 Recursos e Documentação

- [Documentação Angular](https://angular.dev)
- [Angular CLI](https://angular.dev/cli)
- [RxJS](https://rxjs.dev)
- [TypeScript](https://typescriptlang.org)

## 👥 Contribuindo

Consulte o [README principal](../README.md) para guidelines de contribuição.

## 📄 Licença

Projeto educacional - Ver [README principal](../README.md) para mais informações.

---

**Desenvolvido com ❤️ usando Angular 19**
