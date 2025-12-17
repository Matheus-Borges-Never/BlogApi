# Blog API

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![F#](https://img.shields.io/badge/F%23-10.0-378BBA?logo=fsharp)](https://fsharp.org/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Tests](https://img.shields.io/badge/tests-49%20passing-brightgreen)](BlogApi.Tests)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

API RESTful desenvolvida em **.Net** para gerenciamento de posts de blog e comentários.

---

## Índice

- [Tecnologias](#-tecnologias-utilizadas)
- [Pré-requisitos](#-pré-requisitos)
- [Como Executar](#-como-executar-o-projeto)
- [Endpoints da API](#-endpoints-da-api)
- [Testes](#-testando-a-api)
- [Testes Unitários](#-testes-unitarios)
- [Banco de Dados](#-banco-de-dados)
- [Arquitetura](#-arquitetura-do-projeto)
- [Funcionalidades](#-funcionalidades-implementadas)

---

## Tecnologias Utilizadas

- **.NET 10** - Linguagem funcional
- **ASP.NET Core 10** - Framework web
- **Entity Framework Core 10** - ORM
- **SQLite** - Banco de dados (arquivo local)
- **Swagger/OpenAPI** - Documentação interativa da API
- **XUnit + FsUnit** - Framework de testes

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Qualquer editor de código (Visual Studio, VS Code, Rider)

## Como Executar o Projeto

### 1. Clone o repositório
```bash
git clone https://github.com/Matheus-Borges-Never/BlogApi.git
cd BlogApi
```

### 2. Restaure as dependências
```bash
dotnet restore
```

### 3. Execute o projeto
```bash
dotnet run --project BlogApi
```

A API estará disponível em:
- **HTTPS**: https://localhost:7XXX (porta pode variar)
- **HTTP**: http://localhost:5XXX
- **Swagger UI**: https://localhost:7XXX/swagger

> O banco de dados SQLite será criado automaticamente no arquivo `blogapi.db` na raiz do projeto.

## Endpoints da API

> **Total: 9 endpoints**

### Posts

#### 1. Listar todos os posts
```http
GET /api/posts
```

**Resposta:**
```json
[
  {
    "id": 1,
    "titulo": "Bem-vindo ao Blog API",
    "dataCriacao": "2024-01-15T10:30:00Z",
    "numeroComentarios": 2
  }
]
```

#### 2. Criar um novo post
```http
POST /api/posts
Content-Type: application/json

{
  "titulo": "Meu Novo Post",
  "conteudo": "Este é o conteúdo do meu post."
}
```

**Resposta:** 201 Created
```json
{
  "id": 3,
  "titulo": "Meu Novo Post",
  "conteudo": "Este é o conteúdo do meu post.",
  "dataCriacao": "2024-01-15T11:00:00Z",
  "comentarios": []
}
```

#### 3. Obter post específico por ID
```http
GET /api/posts/1
```

**Resposta:**
```json
{
  "id": 1,
  "titulo": "Bem-vindo ao Blog API",
  "conteudo": "Este é o primeiro post do blog...",
  "dataCriacao": "2024-01-15T10:30:00Z",
  "comentarios": [
    {
      "id": 1,
      "autor": "João Silva",
      "texto": "Ótimo post! Muito informativo.",
      "dataCriacao": "2024-01-15T10:45:00Z"
    }
  ]
}
```

#### 4. Atualizar um post
```http
PUT /api/posts/1
Content-Type: application/json

{
  "titulo": "Título Atualizado",
  "conteudo": "Conteúdo atualizado do post."
}
```

**Resposta:** 200 OK
```json
{
  "id": 1,
  "titulo": "Título Atualizado",
  "conteudo": "Conteúdo atualizado do post.",
  "dataCriacao": "2024-01-15T10:30:00Z",
  "comentarios": [...]
}
```

#### 5. Deletar um post 
```http
DELETE /api/posts/1
```

**Resposta:** 204 No Content

> **Atenção**: Deletar um post também deleta todos os comentários associados (cascade delete).

---

### Comentários

#### 6. Adicionar comentário a um post
```http
POST /api/posts/1/comments
Content-Type: application/json

{
  "autor": "Maria Santos",
  "texto": "Excelente conteúdo!"
}
```

**Resposta:** 201 Created
```json
{
  "id": 4,
  "autor": "Maria Santos",
  "texto": "Excelente conteúdo!",
  "dataCriacao": "2024-01-15T11:15:00Z"
}
```

#### 7. Obter comentário específico
```http
GET /api/posts/1/comments/2
```

**Resposta:** 200 OK
```json
{
  "id": 2,
  "autor": "Maria Santos",
  "texto": "Adorei a explicação!",
  "dataCriacao": "2024-01-15T12:00:00Z"
}
```

#### 8. Atualizar um comentário
```http
PUT /api/posts/1/comments/2
Content-Type: application/json

{
  "autor": "Maria Santos",
  "texto": "Comentário atualizado com mais detalhes!"
}
```

**Resposta:** 200 OK
```json
{
  "id": 2,
  "autor": "Maria Santos",
  "texto": "Comentário atualizado com mais detalhes!",
  "dataCriacao": "2024-01-15T12:00:00Z"
}
```

#### 9. Deletar um comentário
```http
DELETE /api/posts/1/comments/2
```

**Resposta:** 204 No Content

---

### Resumo dos Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/posts` | Lista todos os posts |
| POST | `/api/posts` | Cria novo post |
| GET | `/api/posts/{id}` | Obtém post específico |
| PUT | `/api/posts/{id}` | Atualiza post |
| DELETE | `/api/posts/{id}` | Deleta post |
| POST | `/api/posts/{postId}/comments` | Adiciona comentário |
| GET | `/api/posts/{postId}/comments/{commentId}` | Obtém comentário |
| PUT | `/api/posts/{postId}/comments/{commentId}` | Atualiza comentário |
| DELETE | `/api/posts/{postId}/comments/{commentId}` | Deleta comentário |

## Testando a API

### Opção 1: Swagger UI (Recomendado)
1. Execute o projeto
2. Acesse https://localhost:XXXX/swagger
3. Teste todos os endpoints diretamente pelo navegador

### Opção 2: Script PowerShell Automatizado
```bash
.\test-api.ps1
```
Testa todos os 9 endpoints em 30 segundos!

### Opção 3: curl (Manual)

#### Posts
```bash
# Listar posts
curl -k https://localhost:7XXX/api/posts

# Criar post
curl -k -X POST https://localhost:7XXX/api/posts \
  -H "Content-Type: application/json" \
  -d '{"titulo":"Teste","conteudo":"Conteúdo de teste"}'

# Obter post específico
curl -k https://localhost:7XXX/api/posts/1

# Atualizar post
curl -k -X PUT https://localhost:7XXX/api/posts/1 \
  -H "Content-Type: application/json" \
  -d '{"titulo":"Título Atualizado","conteudo":"Conteúdo atualizado"}'

# Deletar post
curl -k -X DELETE https://localhost:7XXX/api/posts/1
```

#### Comentários
```bash
# Adicionar comentário
curl -k -X POST https://localhost:7XXX/api/posts/1/comments \
  -H "Content-Type: application/json" \
  -d '{"autor":"Teste","texto":"Comentário de teste"}'

# Obter comentário específico
curl -k https://localhost:7XXX/api/posts/1/comments/1

# Atualizar comentário
curl -k -X PUT https://localhost:7XXX/api/posts/1/comments/1 \
  -H "Content-Type: application/json" \
  -d '{"autor":"Teste","texto":"Comentário atualizado"}'

# Deletar comentário
curl -k -X DELETE https://localhost:7XXX/api/posts/1/comments/1
```

### Opção 4: Postman/Insomnia
Importe o arquivo `BlogApi.postman_collection.json` incluído no repositório.

---

## ?? Testes Unitários

O projeto inclui **49 testes automatizados** cobrindo toda a funcionalidade da API.

### Executar os Testes

```bash
# Executar todos os testes
cd BlogApi.Tests
dotnet test

# Executar com detalhes
dotnet test --verbosity detailed

# Apenas testes unitários
dotnet test --filter "FullyQualifiedName~UnitTests"

# Apenas testes de integração
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

### Cobertura de Testes

| Categoria | Quantidade | Status |
|-----------|------------|--------|
| **Testes Unitários** | 15 | ? |
| **Testes de Validação** | 15 | ? |
| **Testes de Integração (Posts)** | 9 | ? |
| **Testes de Integração (Comentários)** | 10 | ? |
| **TOTAL** | **49 testes** | ? |

### O que é testado?

#### ? Testes Unitários
- Controller de posts (GET, POST, PUT, DELETE)
- Controller de comentários (GET, POST, PUT, DELETE)
- Cascade delete (deletar post remove comentários)
- Retorno de status codes corretos (200, 201, 204, 404)

#### ? Testes de Validação
- Validação de campos obrigatórios
- Validação de tamanho mínimo e máximo
- Validação de DTOs (Create e Update)

#### ? Testes de Integração
- Requisições HTTP reais para todos os endpoints
- CRUD completo end-to-end
- Validação de status codes
- Serialização/deserialização JSON
- Cascade delete em cenários reais

### Tecnologias de Teste

- **XUnit** - Framework de testes
- **FsUnit** - Assertions em F# idiomático
- **Microsoft.AspNetCore.Mvc.Testing** - Testes de integração
- **Entity Framework InMemory** - Banco em memória
- **FluentAssertions** - Assertions fluentes

> ?? [Documentação completa dos testes](BlogApi.Tests/README.md)

---

## Banco de Dados

### SQLite - Por que essa escolha?

? **Gratuito** - Sem custos de hospedagem  
? **Local** - Arquivo único (`blogapi.db`)  
? **Zero Configuração** - Funciona imediatamente  
? **Portável** - Fácil para testar  
? **Pronto para Produção** - Migração simples para PostgreSQL/SQL Server

### Dados Iniciais (Seed)

O banco é criado automaticamente com dados de exemplo:
- 2 posts de blog
- 3 comentários

### Visualizar o Banco de Dados

Use ferramentas como:
- [DB Browser for SQLite](https://sqlitebrowser.org/)
- [SQLiteStudio](https://sqlitestudio.pl/)
- Extensões do VS Code

## Arquitetura do Projeto

```
BlogApi/
??? Controllers/
?   ??? PostsController.fs      # Endpoints da API (CRUD completo)
??? Data/
?   ??? BlogDbContext.fs        # Contexto do EF Core
??? DTOs/
?   ??? BlogPostDtos.fs         # Data Transfer Objects
??? Models/
?   ??? BlogPost.fs             # Entidades do domínio
??? Program.fs                  # Configuração da aplicação
??? BlogApi.fsproj              # Arquivo do projeto
??? blogapi.db                  # Banco SQLite (gerado)

BlogApi.Tests/
??? Helpers/
?   ??? TestDbContextFactory.fs # Factory para testes
??? UnitTests/
?   ??? PostsControllerTests.fs # Testes do controller
?   ??? ValidationTests.fs      # Testes de validação
??? IntegrationTests/
?   ??? PostsApiTests.fs        # Testes end-to-end posts
?   ??? CommentsApiTests.fs     # Testes end-to-end comentários
??? BlogApi.Tests.fsproj        # Projeto de testes
```

**Padrões implementados:**
- Clean Architecture
- Repository Pattern (via EF Core)
- DTO Pattern
- Dependency Injection
- Async/Await
- CRUD Completo
- Test-Driven Development (TDD)

## Funcionalidades Implementadas

### API
- Modelo de dados BlogPost e Comment
- Relacionamento one-to-many entre Post e Comments
- GET /api/posts - Lista posts com contagem de comentários
- POST /api/posts - Cria novo post
- PUT /api/posts/{id} - Atualiza post
- DELETE /api/posts/{id} - Deleta post
- GET /api/posts/{id} - Obtém post com comentários
- POST /api/posts/{id}/comments - Adiciona comentário
- GET /api/posts/{postId}/comments/{commentId} - Obtém comentário específico
- PUT /api/posts/{postId}/comments/{commentId} - Atualiza comentário
- DELETE /api/posts/{postId}/comments/{commentId} - Deleta comentário

### Qualidade
- ? Validação de dados (ModelState)
- ? Tratamento de erros (404, 400)
- ? Cascade delete (deletar post remove comentários)
- ? Seed data para testes
- ? Documentação Swagger/OpenAPI
- ? Código limpo e organizado
- ? Async/await para operações assíncronas
- ? RESTful seguindo convenções HTTP
- ? **49 testes automatizados** ??
- ? Testes unitários e de integração
- ? 100% dos endpoints testados

---

## Observações

- O projeto foi desenvolvido seguindo **boas práticas** de desenvolvimento
- Código **pronto para produção** (production-ready)
- **Clean Architecture** com separação de responsabilidades
- **Programação funcional** em F# com tipos imutáveis
- **API RESTful** seguindo convenções HTTP
- **CRUD Completo** para posts e comentários
- **Testes Automatizados** - 49 testes cobrindo toda a API
- **Test Coverage** - Testes unitários e de integração

---

## Autor

Desenvolvido por [Matheus Borges Never](https://github.com/Matheus-Borges-Never)

## Licença

Este projeto é de código aberto.

---

<div align="center">

**Feito com ?? em .NET**

**49 testes passando! ?**

[?? Ver Testes](BlogApi.Tests/README.md) | [Rodar Testes](test-api.ps1)

[Voltar ao topo](#blog-api)

</div>
