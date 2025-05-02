# MCP Server

API backend desenvolvida em .NET Core usando Clean Architecture para servir como backend para um MCP Client.

## Tecnologias Utilizadas

- .NET 9.0
- PostgreSQL
- Docker
- Entity Framework Core
- JWT Authentication
- Application Insights
- Swagger/OpenAPI

## Estrutura do Projeto

O projeto segue a Clean Architecture com as seguintes camadas:

- **MCPServer.Domain**: Entidades e interfaces do domínio
- **MCPServer.Application**: Lógica de aplicação
- **MCPServer.Infrastructure**: Implementações de persistência e serviços externos
- **MCPServer.API**: Controllers e configuração da API

## Pré-requisitos

- .NET SDK 9.0 ou superior
- Docker Desktop
- Git (opcional)

## Configuração

1. Clone o repositório:
```bash
git clone https://github.com/wellinfss/mcp-server-dotnet.git
```

2. Navegue até a pasta do projeto:
```bash
cd mcp-server-dotnet
```

3. Inicie o PostgreSQL usando Docker:
```bash
docker-compose up -d
```

4. Navegue até a pasta da API:
```bash
cd src/MCPServer.API
```

5. Execute as migrações do banco de dados:
```bash
dotnet ef database update
```

6. Inicie a aplicação:
```bash
dotnet run
```

## Endpoints

A API estará disponível em `http://localhost:5146` com os seguintes endpoints:

### Autenticação

- **POST /api/auth/register**
  ```json
  {
    "username": "seu_usuario",
    "email": "seu_email@exemplo.com",
    "password": "sua_senha"
  }
  ```

- **POST /api/auth/login**
  ```json
  {
    "email": "seu_email@exemplo.com",
    "password": "sua_senha"
  }
  ```

## Swagger

A documentação da API está disponível em:
```
http://localhost:5146/swagger
```

## Monitoramento

A aplicação está configurada com Application Insights para monitoramento em tempo real.

## Docker

O arquivo `docker-compose.yml` inclui:
- PostgreSQL na porta 5432
- Volume persistente para dados

## Backup e Restauração

Para fazer backup do projeto:
1. Commit todas as alterações
2. Crie um tag com a versão
3. Push para o repositório remoto

Para restaurar:
1. Clone o repositório
2. Checkout na tag desejada
3. Siga os passos de configuração acima 