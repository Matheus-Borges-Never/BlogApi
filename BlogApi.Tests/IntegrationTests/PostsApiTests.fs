namespace BlogApi.Tests.IntegrationTests

open System
open System.Net
open System.Net.Http
open System.Text
open Xunit
open FsUnit.Xunit
open Microsoft.AspNetCore.Mvc.Testing
open Newtonsoft.Json
open BlogApi.DTOs

module PostsApiTestsHelpers =
    let serializeJson obj =
        JsonConvert.SerializeObject(obj)
    
    let deserializeJson<'T> (json: string) =
        JsonConvert.DeserializeObject<'T>(json)
    
    let createJsonContent (obj: obj) =
        new StringContent(serializeJson obj, Encoding.UTF8, "application/json")

type PostsApiTests(factory: WebApplicationFactory<BlogApi.Program>) =
    let client = factory.CreateClient()
    
    interface IClassFixture<WebApplicationFactory<BlogApi.Program>>
    
    [<Fact>]
    member _.``GET api/posts deve retornar 200 OK`` () =
        async {
            // Act
            let! response = client.GetAsync("/api/posts") |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.OK
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``GET api/posts deve retornar lista de posts`` () =
        async {
            // Act
            let! response = client.GetAsync("/api/posts") |> Async.AwaitTask
            let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.OK
            let posts = PostsApiTestsHelpers.deserializeJson<BlogPostListItemDto list> content
            posts |> should not' (be Empty)
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``POST api/posts deve criar novo post`` () =
        async {
            // Arrange
            let newPost = {
                CreateBlogPostDto.Titulo = "Post de Teste Integração"
                CreateBlogPostDto.Conteudo = "Conteúdo do teste de integração"
            }
            let content = PostsApiTestsHelpers.createJsonContent newPost
            
            // Act
            let! response = client.PostAsync("/api/posts", content) |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.Created
            let! responseContent = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            let createdPost = PostsApiTestsHelpers.deserializeJson<BlogPostDetailDto> responseContent
            createdPost.Titulo |> should equal "Post de Teste Integração"
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``POST api/posts deve retornar 400 com dados invalidos`` () =
        async {
            // Arrange
            let invalidPost = {
                CreateBlogPostDto.Titulo = ""
                CreateBlogPostDto.Conteudo = "Conteúdo válido"
            }
            let content = PostsApiTestsHelpers.createJsonContent invalidPost
            
            // Act
            let! response = client.PostAsync("/api/posts", content) |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.BadRequest
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``GET api/posts/id deve retornar post especifico`` () =
        async {
            // Arrange - Criar um post primeiro
            let newPost = {
                CreateBlogPostDto.Titulo = "Post para GET"
                CreateBlogPostDto.Conteudo = "Conteúdo do post para GET"
            }
            let content = PostsApiTestsHelpers.createJsonContent newPost
            let! createResponse = client.PostAsync("/api/posts", content) |> Async.AwaitTask
            let! createContent = createResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let createdPost = PostsApiTestsHelpers.deserializeJson<BlogPostDetailDto> createContent
            
            // Act
            let! response = client.GetAsync($"/api/posts/{createdPost.Id}") |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.OK
            let! responseContent = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            let post = PostsApiTestsHelpers.deserializeJson<BlogPostDetailDto> responseContent
            post.Titulo |> should equal "Post para GET"
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``GET api/posts/999 deve retornar 404`` () =
        async {
            // Act
            let! response = client.GetAsync("/api/posts/999999") |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.NotFound
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``PUT api/posts/id deve atualizar post`` () =
        async {
            // Arrange - Criar um post primeiro
            let newPost = {
                CreateBlogPostDto.Titulo = "Post para PUT"
                CreateBlogPostDto.Conteudo = "Conteúdo original"
            }
            let content = PostsApiTestsHelpers.createJsonContent newPost
            let! createResponse = client.PostAsync("/api/posts", content) |> Async.AwaitTask
            let! createContent = createResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let createdPost = PostsApiTestsHelpers.deserializeJson<BlogPostDetailDto> createContent
            
            // Act
            let updatePost = {
                UpdateBlogPostDto.Titulo = "Post Atualizado"
                UpdateBlogPostDto.Conteudo = "Conteúdo atualizado"
            }
            let updateContent = PostsApiTestsHelpers.createJsonContent updatePost
            let! updateResponse = client.PutAsync($"/api/posts/{createdPost.Id}", updateContent) |> Async.AwaitTask
            
            // Assert
            updateResponse.StatusCode |> should equal HttpStatusCode.OK
            let! responseContent = updateResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let updatedPost = PostsApiTestsHelpers.deserializeJson<BlogPostDetailDto> responseContent
            updatedPost.Titulo |> should equal "Post Atualizado"
            updatedPost.Conteudo |> should equal "Conteúdo atualizado"
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``DELETE api/posts/id deve deletar post`` () =
        async {
            // Arrange - Criar um post primeiro
            let newPost = {
                CreateBlogPostDto.Titulo = "Post para DELETE"
                CreateBlogPostDto.Conteudo = "Será deletado"
            }
            let content = PostsApiTestsHelpers.createJsonContent newPost
            let! createResponse = client.PostAsync("/api/posts", content) |> Async.AwaitTask
            let! createContent = createResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let createdPost = PostsApiTestsHelpers.deserializeJson<BlogPostDetailDto> createContent
            
            // Act
            let! deleteResponse = client.DeleteAsync($"/api/posts/{createdPost.Id}") |> Async.AwaitTask
            
            // Assert
            deleteResponse.StatusCode |> should equal HttpStatusCode.NoContent
            
            // Verificar que foi deletado
            let! getResponse = client.GetAsync($"/api/posts/{createdPost.Id}") |> Async.AwaitTask
            getResponse.StatusCode |> should equal HttpStatusCode.NotFound
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``CRUD completo de post deve funcionar`` () =
        async {
            // CREATE
            let newPost = {
                CreateBlogPostDto.Titulo = "Post CRUD Completo"
                CreateBlogPostDto.Conteudo = "Teste completo"
            }
            let content = PostsApiTestsHelpers.createJsonContent newPost
            let! createResponse = client.PostAsync("/api/posts", content) |> Async.AwaitTask
            createResponse.StatusCode |> should equal HttpStatusCode.Created
            let! createContent = createResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let createdPost = PostsApiTestsHelpers.deserializeJson<BlogPostDetailDto> createContent
            
            // READ
            let! readResponse = client.GetAsync($"/api/posts/{createdPost.Id}") |> Async.AwaitTask
            readResponse.StatusCode |> should equal HttpStatusCode.OK
            
            // UPDATE
            let updatePost = {
                UpdateBlogPostDto.Titulo = "Post Atualizado CRUD"
                UpdateBlogPostDto.Conteudo = "Conteúdo atualizado CRUD"
            }
            let updateContent = PostsApiTestsHelpers.createJsonContent updatePost
            let! updateResponse = client.PutAsync($"/api/posts/{createdPost.Id}", updateContent) |> Async.AwaitTask
            updateResponse.StatusCode |> should equal HttpStatusCode.OK
            
            // DELETE
            let! deleteResponse = client.DeleteAsync($"/api/posts/{createdPost.Id}") |> Async.AwaitTask
            deleteResponse.StatusCode |> should equal HttpStatusCode.NoContent
            
            // Verificar deleção
            let! finalGetResponse = client.GetAsync($"/api/posts/{createdPost.Id}") |> Async.AwaitTask
            finalGetResponse.StatusCode |> should equal HttpStatusCode.NotFound
        } |> Async.RunSynchronously
