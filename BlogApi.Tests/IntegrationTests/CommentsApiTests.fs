namespace BlogApi.Tests.IntegrationTests

open System.Net
open System.Net.Http
open System.Text
open Xunit
open FsUnit.Xunit
open Microsoft.AspNetCore.Mvc.Testing
open Newtonsoft.Json
open BlogApi.DTOs

module CommentsApiTestsHelpers =
    let serializeJson obj =
        JsonConvert.SerializeObject(obj)
    
    let deserializeJson<'T> (json: string) =
        JsonConvert.DeserializeObject<'T>(json)
    
    let createJsonContent (obj: obj) =
        new StringContent(serializeJson obj, Encoding.UTF8, "application/json")

type CommentsApiTests(factory: WebApplicationFactory<BlogApi.Program>) =
    let client = factory.CreateClient()
    
    let createTestPost () =
        async {
            let newPost = {
                CreateBlogPostDto.Titulo = "Post para Comentários"
                CreateBlogPostDto.Conteudo = "Post de teste para comentários"
            }
            let content = CommentsApiTestsHelpers.createJsonContent newPost
            let! response = client.PostAsync("/api/posts", content) |> Async.AwaitTask
            let! responseContent = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            return CommentsApiTestsHelpers.deserializeJson<BlogPostDetailDto> responseContent
        }
    
    interface IClassFixture<WebApplicationFactory<BlogApi.Program>>
    
    [<Fact>]
    member _.``POST api/posts/id/comments deve adicionar comentario`` () =
        async {
            // Arrange - Criar post
            let! post = createTestPost()
            
            let newComment = {
                CreateCommentDto.Autor = "Testador"
                CreateCommentDto.Texto = "Comentário de teste"
            }
            let content = CommentsApiTestsHelpers.createJsonContent newComment
            
            // Act
            let! response = client.PostAsync($"/api/posts/{post.Id}/comments", content) |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.Created
            let! responseContent = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            let comment = CommentsApiTestsHelpers.deserializeJson<CommentDto> responseContent
            comment.Autor |> should equal "Testador"
            comment.Texto |> should equal "Comentário de teste"
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``POST api/posts/999/comments deve retornar 404`` () =
        async {
            // Arrange
            let newComment = {
                CreateCommentDto.Autor = "Testador"
                CreateCommentDto.Texto = "Comentário"
            }
            let content = CommentsApiTestsHelpers.createJsonContent newComment
            
            // Act
            let! response = client.PostAsync("/api/posts/999999/comments", content) |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.NotFound
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``POST comentario com dados invalidos deve retornar 400`` () =
        async {
            // Arrange
            let! post = createTestPost()
            
            let invalidComment = {
                CreateCommentDto.Autor = ""
                CreateCommentDto.Texto = "Comentário válido"
            }
            let content = CommentsApiTestsHelpers.createJsonContent invalidComment
            
            // Act
            let! response = client.PostAsync($"/api/posts/{post.Id}/comments", content) |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.BadRequest
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``GET api/posts/id/comments/commentId deve retornar comentario`` () =
        async {
            // Arrange - Criar post e comentário
            let! post = createTestPost()
            
            let newComment = {
                CreateCommentDto.Autor = "Testador GET"
                CreateCommentDto.Texto = "Comentário para GET"
            }
            let content = CommentsApiTestsHelpers.createJsonContent newComment
            let! createResponse = client.PostAsync($"/api/posts/{post.Id}/comments", content) |> Async.AwaitTask
            let! createContent = createResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let createdComment = CommentsApiTestsHelpers.deserializeJson<CommentDto> createContent
            
            // Act
            let! response = client.GetAsync($"/api/posts/{post.Id}/comments/{createdComment.Id}") |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.OK
            let! responseContent = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            let comment = CommentsApiTestsHelpers.deserializeJson<CommentDto> responseContent
            comment.Autor |> should equal "Testador GET"
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``GET comentario inexistente deve retornar 404`` () =
        async {
            // Arrange
            let! post = createTestPost()
            
            // Act
            let! response = client.GetAsync($"/api/posts/{post.Id}/comments/999999") |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.NotFound
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``PUT api/posts/id/comments/commentId deve atualizar comentario`` () =
        async {
            // Arrange - Criar post e comentário
            let! post = createTestPost()
            
            let newComment = {
                CreateCommentDto.Autor = "Testador Original"
                CreateCommentDto.Texto = "Comentário original"
            }
            let content = CommentsApiTestsHelpers.createJsonContent newComment
            let! createResponse = client.PostAsync($"/api/posts/{post.Id}/comments", content) |> Async.AwaitTask
            let! createContent = createResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let createdComment = CommentsApiTestsHelpers.deserializeJson<CommentDto> createContent
            
            // Act
            let updateComment = {
                UpdateCommentDto.Autor = "Testador Atualizado"
                UpdateCommentDto.Texto = "Comentário atualizado"
            }
            let updateContent = CommentsApiTestsHelpers.createJsonContent updateComment
            let! updateResponse = client.PutAsync($"/api/posts/{post.Id}/comments/{createdComment.Id}", updateContent) |> Async.AwaitTask
            
            // Assert
            updateResponse.StatusCode |> should equal HttpStatusCode.OK
            let! responseContent = updateResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let updatedComment = CommentsApiTestsHelpers.deserializeJson<CommentDto> responseContent
            updatedComment.Autor |> should equal "Testador Atualizado"
            updatedComment.Texto |> should equal "Comentário atualizado"
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``DELETE api/posts/id/comments/commentId deve deletar comentario`` () =
        async {
            // Arrange - Criar post e comentário
            let! post = createTestPost()
            
            let newComment = {
                CreateCommentDto.Autor = "Testador DELETE"
                CreateCommentDto.Texto = "Será deletado"
            }
            let content = CommentsApiTestsHelpers.createJsonContent newComment
            let! createResponse = client.PostAsync($"/api/posts/{post.Id}/comments", content) |> Async.AwaitTask
            let! createContent = createResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let createdComment = CommentsApiTestsHelpers.deserializeJson<CommentDto> createContent
            
            // Act
            let! deleteResponse = client.DeleteAsync($"/api/posts/{post.Id}/comments/{createdComment.Id}") |> Async.AwaitTask
            
            // Assert
            deleteResponse.StatusCode |> should equal HttpStatusCode.NoContent
            
            // Verificar que foi deletado
            let! getResponse = client.GetAsync($"/api/posts/{post.Id}/comments/{createdComment.Id}") |> Async.AwaitTask
            getResponse.StatusCode |> should equal HttpStatusCode.NotFound
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``CRUD completo de comentario deve funcionar`` () =
        async {
            // Criar post
            let! post = createTestPost()
            
            // CREATE comentário
            let newComment = {
                CreateCommentDto.Autor = "Testador CRUD"
                CreateCommentDto.Texto = "Comentário CRUD completo"
            }
            let content = CommentsApiTestsHelpers.createJsonContent newComment
            let! createResponse = client.PostAsync($"/api/posts/{post.Id}/comments", content) |> Async.AwaitTask
            createResponse.StatusCode |> should equal HttpStatusCode.Created
            let! createContent = createResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let createdComment = CommentsApiTestsHelpers.deserializeJson<CommentDto> createContent
            
            // READ comentário
            let! readResponse = client.GetAsync($"/api/posts/{post.Id}/comments/{createdComment.Id}") |> Async.AwaitTask
            readResponse.StatusCode |> should equal HttpStatusCode.OK
            
            // UPDATE comentário
            let updateComment = {
                UpdateCommentDto.Autor = "Testador CRUD Atualizado"
                UpdateCommentDto.Texto = "Comentário CRUD atualizado"
            }
            let updateContent = CommentsApiTestsHelpers.createJsonContent updateComment
            let! updateResponse = client.PutAsync($"/api/posts/{post.Id}/comments/{createdComment.Id}", updateContent) |> Async.AwaitTask
            updateResponse.StatusCode |> should equal HttpStatusCode.OK
            
            // DELETE comentário
            let! deleteResponse = client.DeleteAsync($"/api/posts/{post.Id}/comments/{createdComment.Id}") |> Async.AwaitTask
            deleteResponse.StatusCode |> should equal HttpStatusCode.NoContent
            
            // Verificar deleção
            let! finalGetResponse = client.GetAsync($"/api/posts/{post.Id}/comments/{createdComment.Id}") |> Async.AwaitTask
            finalGetResponse.StatusCode |> should equal HttpStatusCode.NotFound
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``Deletar post deve deletar comentarios em cascade`` () =
        async {
            // Arrange - Criar post com comentário
            let! post = createTestPost()
            
            let newComment = {
                CreateCommentDto.Autor = "Testador Cascade"
                CreateCommentDto.Texto = "Comentário que será deletado com post"
            }
            let content = CommentsApiTestsHelpers.createJsonContent newComment
            let! createCommentResponse = client.PostAsync($"/api/posts/{post.Id}/comments", content) |> Async.AwaitTask
            let! commentContent = createCommentResponse.Content.ReadAsStringAsync() |> Async.AwaitTask
            let createdComment = CommentsApiTestsHelpers.deserializeJson<CommentDto> commentContent
            
            // Act - Deletar o post
            let! deletePostResponse = client.DeleteAsync($"/api/posts/{post.Id}") |> Async.AwaitTask
            
            // Assert
            deletePostResponse.StatusCode |> should equal HttpStatusCode.NoContent
            
            // Verificar que o post foi deletado
            let! getPostResponse = client.GetAsync($"/api/posts/{post.Id}") |> Async.AwaitTask
            getPostResponse.StatusCode |> should equal HttpStatusCode.NotFound
            
            // Verificar que o comentário também foi deletado (cascade)
            let! getCommentResponse = client.GetAsync($"/api/posts/{post.Id}/comments/{createdComment.Id}") |> Async.AwaitTask
            getCommentResponse.StatusCode |> should equal HttpStatusCode.NotFound
        } |> Async.RunSynchronously
    
    [<Fact>]
    member _.``Post com multiplos comentarios deve retornar todos`` () =
        async {
            // Arrange - Criar post
            let! post = createTestPost()
            
            // Adicionar múltiplos comentários
            for i in 1..3 do
                let newComment = {
                    CreateCommentDto.Autor = $"Testador {i}"
                    CreateCommentDto.Texto = $"Comentário {i}"
                }
                let content = CommentsApiTestsHelpers.createJsonContent newComment
                let! _ = client.PostAsync($"/api/posts/{post.Id}/comments", content) |> Async.AwaitTask
                ()
            
            // Act - Buscar o post com todos os comentários
            let! response = client.GetAsync($"/api/posts/{post.Id}") |> Async.AwaitTask
            
            // Assert
            response.StatusCode |> should equal HttpStatusCode.OK
            let! responseContent = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            let postDetail = CommentsApiTestsHelpers.deserializeJson<BlogPostDetailDto> responseContent
            postDetail.Comentarios |> List.length |> should equal 3
        } |> Async.RunSynchronously
