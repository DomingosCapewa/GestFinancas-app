# GestFinancas-Api

**GestFinancas-Api** é a API desenvolvida para suportar as operações do sistema de gerenciamento financeiro **GestFinancas**. Essa API foi construída utilizando **C#** e segue boas práticas de desenvolvimento para garantir escalabilidade, segurança e manutenibilidade.

## Funcionalidades

- **Gerenciamento de Usuários**:
  - Cadastro, login e autenticação de usuários.
  - Recuperação de senha.

- **Gerenciamento de Transações**:
  - Adição, edição e exclusão de receitas e despesas.
  - Consultas filtradas por data, categoria e valor.

- **Relatórios Financeiros**:
  - Geração de relatórios para análise de receitas e despesas.
  - Exportação de dados.

## Tecnologias Utilizadas

- **Linguagem**: C#
- **Framework**: .NET 6.
- **Banco de Dados**: SQL Server
- **Autenticação**: JWT (JSON Web Tokens) para autenticação segura.
- **Outras Dependências**: EF.

## Como Configurar o Projeto

Siga as instruções abaixo para configurar e executar a API localmente.

### Pré-requisitos

- **SDK do .NET** (recomendado .NET 6 ou superior) instalado.
- Banco de dados configurado e rodando (ex.: SQL Server).
- Ferramenta como Postman ou Insomnia para testar endpoints.

### Passos para Configuração

1. Clone o repositório:
   ```bash
   git clone https://github.com/DomingosCapewa/GestFinancas-Api.git
   ```

2. Navegue até o diretório do projeto:
   ```bash
   cd GestFinancas-Api
   ```

3. Configure a string de conexão com o banco de dados no arquivo `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=SEU_SERVIDOR;Database=SEU_BANCO_DE_DADOS;User Id=SEU_USUARIO;Password=SUA_SENHA;"
     }
   }
   ```

4. Restaure as dependências do projeto:
   ```bash
   dotnet restore
   ```

5. Execute as migrações para criar o esquema do banco de dados:
   ```bash
   dotnet ef database update
   ```

6. Inicie o servidor:
   ```bash
   dotnet run
   ```

7. Acesse a API no navegador ou em uma ferramenta como Postman:
   ```
   http://localhost:5000
   ```

## Estrutura do Projeto

Aqui está uma visão geral da estrutura principal do projeto:

```
GestFinancas-Api/
├── Controllers/       # Controladores da API
├── Models/            # Modelos de dados
├── Repositories/      # Repositórios para acesso ao banco de dados
├── Services/          # Regras de negócio e lógica da aplicação
├── appsettings.json   # Configurações do aplicativo
├── Program.cs         # Configuração e inicialização do aplicativo
├── Startup.cs         # Configuração de middlewares e serviços
```

## Endpoints Principais

- **Autenticação**:
  - `POST /api/auth/register`: Registrar um novo usuário.
  - `POST /api/auth/login`: Autenticar e obter token JWT.

- **Transações**:
  - `GET /api/transactions`: Listar todas as transações.
  - `POST /api/transactions`: Criar uma nova transação.
  - `PUT /api/transactions/{id}`: Atualizar uma transação.
  - `DELETE /api/transactions/{id}`: Excluir uma transação.

- **Relatórios**:
  - `GET /api/reports/summary`: Obter resumo financeiro.

## Contribuição

Contribuições são bem-vindas! Siga os passos abaixo para contribuir:

1. Faça um fork deste repositório.
2. Crie uma branch para sua feature:
   ```bash
   git checkout -b minha-feature
   ```
3. Commit suas alterações:
   ```bash
   git commit -m "Minha nova feature"
   ```
4. Envie sua branch:
   ```bash
   git push origin minha-feature
   ```
5. Abra um Pull Request.

## Licença

Este projeto atualmente não possui uma licença. Entre em contato com o proprietário do repositório para mais detalhes.
