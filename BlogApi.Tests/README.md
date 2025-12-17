# 🧪 Testes da Blog API

## 📋 Visão Geral

Este projeto contém testes unitários e de integração completos para a Blog API.

### Tipos de Testes

- **Testes Unitários** - Testam componentes isolados (controllers, validações)
- **Testes de Integração** - Testam a API completa com requests HTTP reais

---

## 🚀 Como Executar os Testes

### Executar todos os testes
```bash
cd BlogApi.Tests
dotnet test
```

### Executar com detalhes verbose
```bash
dotnet test --verbosity detailed
```

### Executar apenas testes unitários
```bash
dotnet test --filter "FullyQualifiedName~UnitTests"
```

### Executar apenas testes de integração
```bash
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

### Executar com cobertura de código
```bash
dotnet test /p:CollectCoverage=true
```

---

## 📊 Estrutura dos Testes

```
BlogApi.Tests/
├── Helpers/
│   └── TestDbContextFactory.fs      # Factory para criar DbContext de teste
├── UnitTests/
│   ├── PostsControllerTests.fs      # Testes do controller de posts
│   └── ValidationTests.fs           # Testes de validação de DTOs
├── IntegrationTests/
│   ├── PostsApiTests.fs            # Testes end-to-end de posts
│   └── CommentsApiTests.fs         # Testes end-to-end de comentários
└── BlogApi.Tests.fsproj            # Arquivo do projeto
```

---

## ✅ Cobertura de Testes

### Testes Unitários (PostsControllerTests.fs)

#### Posts
- ✅ `GetAllPosts` - Lista todos os posts
- ✅ `GetAllPosts` - Inclui contagem de comentários
- ✅ `GetPostById` - Retorna post quando existe
- ✅ `GetPostById` - Retorna 404 quando não existe
- ✅ `CreatePost` - Cria novo post com sucesso
- ✅ `UpdatePost` - Atualiza post existente
- ✅ `UpdatePost` - Retorna 404 quando não existe
- ✅ `DeletePost` - Deleta post com sucesso
- ✅ `DeletePost` - Retorna 404 quando não existe

#### Comentários
- ✅ `AddComment` - Adiciona comentário com sucesso
- ✅ `AddComment` - Retorna 404 quando post não existe
- ✅ `GetCommentById` - Retorna comentário quando existe
- ✅ `UpdateComment` - Atualiza comentário existente
- ✅ `DeleteComment` - Deleta comentário com sucesso

#### Cascade Delete
- ✅ `DeletePost` - Deleta comentários em cascade

**Total: 15 testes unitários** ✅

---

### Testes de Validação (ValidationTests.fs)

#### CreateBlogPostDto
- ✅ Válido com dados corretos
- ✅ Falha com título vazio
- ✅ Falha com título muito longo (>200)
- ✅ Falha com conteúdo vazio
- ✅ Aceita título no limite mínimo (1 char)
- ✅ Aceita título no limite máximo (200 chars)

#### CreateCommentDto
- ✅ Válido com dados corretos
- ✅ Falha com autor vazio
- ✅ Falha com autor muito longo (>100)
- ✅ Falha com texto vazio
- ✅ Falha com texto muito longo (>1000)
- ✅ Aceita autor no limite mínimo (1 char)
- ✅ Aceita autor no limite máximo (100 chars)

#### UpdateDTOs
- ✅ UpdateBlogPostDto válido
- ✅ UpdateCommentDto válido

**Total: 15 testes de validação** ✅

---

### Testes de Integração - Posts (PostsApiTests.fs)

- ✅ `GET /api/posts` - Retorna 200 OK
- ✅ `GET /api/posts` - Retorna lista de posts
- ✅ `POST /api/posts` - Cria novo post (201)
- ✅ `POST /api/posts` - Retorna 400 com dados inválidos
- ✅ `GET /api/posts/{id}` - Retorna post específico
- ✅ `GET /api/posts/999` - Retorna 404
- ✅ `PUT /api/posts/{id}` - Atualiza post (200)
- ✅ `DELETE /api/posts/{id}` - Deleta post (204)
- ✅ CRUD completo funciona end-to-end

**Total: 9 testes de integração (Posts)** ✅

---

### Testes de Integração - Comentários (CommentsApiTests.fs)

- ✅ `POST /api/posts/{id}/comments` - Adiciona comentário (201)
- ✅ `POST /api/posts/999/comments` - Retorna 404
- ✅ `POST` comentário inválido - Retorna 400
- ✅ `GET /api/posts/{id}/comments/{cId}` - Retorna comentário
- ✅ `GET` comentário inexistente - Retorna 404
- ✅ `PUT /api/posts/{id}/comments/{cId}` - Atualiza comentário (200)
- ✅ `DELETE /api/posts/{id}/comments/{cId}` - Deleta comentário (204)
- ✅ CRUD completo de comentário funciona
- ✅ Deletar post deleta comentários (cascade)
- ✅ Post com múltiplos comentários retorna todos

**Total: 10 testes de integração (Comentários)** ✅

---

## 📈 Resumo Total

| Categoria | Quantidade | Status |
|-----------|------------|--------|
| **Testes Unitários** | 15 | ✅ |
| **Testes de Validação** | 15 | ✅ |
| **Testes de Integração (Posts)** | 9 | ✅ |
| **Testes de Integração (Comentários)** | 10 | ✅ |
| **TOTAL** | **49 testes** | ✅ |

---

## 🛠️ Tecnologias de Teste

- **XUnit** - Framework de testes
- **FsUnit** - Assertions em F# idiomático
- **Microsoft.AspNetCore.Mvc.Testing** - Testes de integração
- **Entity Framework InMemory** - Banco de dados em memória
- **FluentAssertions** - Assertions fluentes

---

## 💡 Padrões de Teste Utilizados

### Arrange-Act-Assert (AAA)
```fsharp
[<Fact>]
let ``teste exemplo`` () =
    // Arrange - Preparar dados
    let context = createTestContext()
    
    // Act - Executar ação
    let result = controller.Method()
    
    // Assert - Verificar resultado
    result |> should equal expected
```

### In-Memory Database
```fsharp
let createInMemoryContext (databaseName: string) =
    let options = 
        DbContextOptionsBuilder<BlogDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options
    new BlogDbContext(options)
```

### WebApplicationFactory para Integração
```fsharp
type PostsApiTests(factory: WebApplicationFactory<BlogApi.Program>) =
    interface IClassFixture<WebApplicationFactory<BlogApi.Program>>
    let client = factory.CreateClient()
```

---

## 🧪 Exemplos de Testes

### Teste Unitário Simples
```fsharp
[<Fact>]
let ``GetAllPosts deve retornar lista de posts`` () =
    // Arrange
    let context = TestDbContextFactory.createSeededContext "TestDb"
    let controller = PostsController(context)
    
    // Act
    let result = controller.GetAllPosts().Result
    
    // Assert
    result |> should be instanceOfType<OkObjectResult>
    context.Dispose()
```

### Teste de Validação
```fsharp
[<Fact>]
let ``CreateBlogPostDto deve falhar com titulo vazio`` () =
    // Arrange
    let dto = { Titulo = ""; Conteudo = "Válido" }
    
    // Act
    let (isValid, errors) = validateModel dto
    
    // Assert
    isValid |> should equal false
    errors.Count |> should be (greaterThan 0)
```

### Teste de Integração
```fsharp
[<Fact>]
member _.``POST api/posts deve criar novo post`` () =
    async {
        // Arrange
        let newPost = { Titulo = "Teste"; Conteudo = "Conteúdo" }
        let content = createJsonContent newPost
        
        // Act
        let! response = client.PostAsync("/api/posts", content) |> Async.AwaitTask
        
        // Assert
        response.StatusCode |> should equal HttpStatusCode.Created
    } |> Async.RunSynchronously
```

---

## 📝 Comandos Úteis

### Executar testes específicos
```bash
# Por nome
dotnet test --filter "DisplayName~GetAllPosts"

# Por categoria
dotnet test --filter "Category=Unit"
```

### Ver resultados detalhados
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Executar testes em paralelo
```bash
dotnet test --parallel
```

### Gerar relatório HTML
```bash
dotnet test --logger "html;logfilename=testResults.html"
```

---

## 🎯 Cenários Testados

### ✅ Cenários de Sucesso
- Criar, ler, atualizar e deletar posts
- Criar, ler, atualizar e deletar comentários
- Listar posts com contagem de comentários
- Retornar post com todos os comentários

### ✅ Cenários de Erro
- 404 quando recurso não existe
- 400 quando dados são inválidos
- Validações de tamanho de campos
- Validações de campos obrigatórios

### ✅ Cenários Especiais
- Cascade delete (deletar post remove comentários)
- Posts sem comentários
- Posts com múltiplos comentários
- Ordenação de comentários

---

## 📚 Referências

- [XUnit Documentation](https://xunit.net/)
- [FsUnit GitHub](https://github.com/fsprojects/FsUnit)
- [ASP.NET Core Testing](https://docs.microsoft.com/aspnet/core/test/)
- [EF Core In-Memory Testing](https://docs.microsoft.com/ef/core/testing/)

---

<div align="center">

**49 testes implementados e passando! ✅**

[⬆ Voltar ao topo](#-testes-da-blog-api)

</div>
