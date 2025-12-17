namespace BlogApi.Tests.UnitTests

open System
open Xunit
open FsUnit.Xunit
open Microsoft.AspNetCore.Mvc
open BlogApi.Controllers
open BlogApi.DTOs
open BlogApi.Tests.Helpers

module PostsControllerTests =
    
    [<Fact>]
    let ``GetAllPosts deve retornar lista de posts`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "GetAllPostsTest"
        let controller = PostsController(context)
        
        // Act
        let result = controller.GetAllPosts().Result
        
        // Assert
        result |> should be instanceOfType<OkObjectResult>
        let okResult = result :?> OkObjectResult
        let posts = okResult.Value :?> seq<BlogPostListItemDto>
        posts |> Seq.length |> should equal 2
        
        context.Dispose()
    
    [<Fact>]
    let ``GetAllPosts deve incluir contagem de comentarios`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "GetAllPostsWithCommentsTest"
        let controller = PostsController(context)
        
        // Act
        let result = controller.GetAllPosts().Result
        
        // Assert
        let okResult = result :?> OkObjectResult
        let posts = okResult.Value :?> seq<BlogPostListItemDto> |> Seq.toList
        let firstPost = posts |> List.head
        firstPost.NumeroComentarios |> should equal 2
        
        context.Dispose()
    
    [<Fact>]
    let ``GetPostById deve retornar post quando existe`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "GetPostByIdTest"
        let controller = PostsController(context)
        
        // Act
        let result = controller.GetPostById(1).Result
        
        // Assert
        result |> should be instanceOfType<OkObjectResult>
        let okResult = result :?> OkObjectResult
        let post = okResult.Value :?> BlogPostDetailDto
        post.Id |> should equal 1
        post.Titulo |> should equal "Post de Teste 1"
        post.Comentarios |> List.length |> should equal 2
        
        context.Dispose()
    
    [<Fact>]
    let ``GetPostById deve retornar 404 quando post nao existe`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "GetPostByIdNotFoundTest"
        let controller = PostsController(context)
        
        // Act
        let result = controller.GetPostById(999).Result
        
        // Assert
        result |> should be instanceOfType<NotFoundObjectResult>
        
        context.Dispose()
    
    [<Fact>]
    let ``CreatePost deve criar novo post com sucesso`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "CreatePostTest"
        let controller = PostsController(context)
        let createDto = {
            CreateBlogPostDto.Titulo = "Novo Post de Teste"
            CreateBlogPostDto.Conteudo = "Conteúdo do novo post"
        }
        
        // Act
        let result = controller.CreatePost(createDto).Result
        
        // Assert
        result |> should be instanceOfType<CreatedAtActionResult>
        let createdResult = result :?> CreatedAtActionResult
        let post = createdResult.Value :?> BlogPostDetailDto
        post.Titulo |> should equal "Novo Post de Teste"
        post.Conteudo |> should equal "Conteúdo do novo post"
        
        context.Dispose()
    
    [<Fact>]
    let ``UpdatePost deve atualizar post existente`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "UpdatePostTest"
        let controller = PostsController(context)
        let updateDto = {
            UpdateBlogPostDto.Titulo = "Post Atualizado"
            UpdateBlogPostDto.Conteudo = "Conteúdo atualizado"
        }
        
        // Act
        let result = controller.UpdatePost(1, updateDto).Result
        
        // Assert
        result |> should be instanceOfType<OkObjectResult>
        let okResult = result :?> OkObjectResult
        let post = okResult.Value :?> BlogPostDetailDto
        post.Titulo |> should equal "Post Atualizado"
        post.Conteudo |> should equal "Conteúdo atualizado"
        
        context.Dispose()
    
    [<Fact>]
    let ``UpdatePost deve retornar 404 quando post nao existe`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "UpdatePostNotFoundTest"
        let controller = PostsController(context)
        let updateDto = {
            UpdateBlogPostDto.Titulo = "Post Atualizado"
            UpdateBlogPostDto.Conteudo = "Conteúdo atualizado"
        }
        
        // Act
        let result = controller.UpdatePost(999, updateDto).Result
        
        // Assert
        result |> should be instanceOfType<NotFoundObjectResult>
        
        context.Dispose()
    
    [<Fact>]
    let ``DeletePost deve deletar post com sucesso`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "DeletePostTest"
        let controller = PostsController(context)
        
        // Act
        let result = controller.DeletePost(1).Result
        
        // Assert
        result |> should be instanceOfType<NoContentResult>
        
        // Verificar se foi deletado
        let getResult = controller.GetPostById(1).Result
        getResult |> should be instanceOfType<NotFoundObjectResult>
        
        context.Dispose()
    
    [<Fact>]
    let ``DeletePost deve retornar 404 quando post nao existe`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "DeletePostNotFoundTest"
        let controller = PostsController(context)
        
        // Act
        let result = controller.DeletePost(999).Result
        
        // Assert
        result |> should be instanceOfType<NotFoundObjectResult>
        
        context.Dispose()
    
    [<Fact>]
    let ``AddComment deve adicionar comentario com sucesso`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "AddCommentTest"
        let controller = PostsController(context)
        let commentDto = {
            CreateCommentDto.Autor = "Novo Testador"
            CreateCommentDto.Texto = "Novo comentário de teste"
        }
        
        // Act
        let result = controller.AddComment(1, commentDto).Result
        
        // Assert
        result |> should be instanceOfType<CreatedResult>
        let createdResult = result :?> CreatedResult
        let comment = createdResult.Value :?> CommentDto
        comment.Autor |> should equal "Novo Testador"
        comment.Texto |> should equal "Novo comentário de teste"
        
        context.Dispose()
    
    [<Fact>]
    let ``AddComment deve retornar 404 quando post nao existe`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "AddCommentNotFoundTest"
        let controller = PostsController(context)
        let commentDto = {
            CreateCommentDto.Autor = "Testador"
            CreateCommentDto.Texto = "Comentário"
        }
        
        // Act
        let result = controller.AddComment(999, commentDto).Result
        
        // Assert
        result |> should be instanceOfType<NotFoundObjectResult>
        
        context.Dispose()
    
    [<Fact>]
    let ``GetCommentById deve retornar comentario quando existe`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "GetCommentByIdTest"
        let controller = PostsController(context)
        
        // Act
        let result = controller.GetCommentById(1, 1).Result
        
        // Assert
        result |> should be instanceOfType<OkObjectResult>
        let okResult = result :?> OkObjectResult
        let comment = okResult.Value :?> CommentDto
        comment.Id |> should equal 1
        comment.Autor |> should equal "Testador 1"
        
        context.Dispose()
    
    [<Fact>]
    let ``UpdateComment deve atualizar comentario existente`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "UpdateCommentTest"
        let controller = PostsController(context)
        let updateDto = {
            UpdateCommentDto.Autor = "Testador Atualizado"
            UpdateCommentDto.Texto = "Comentário atualizado"
        }
        
        // Act
        let result = controller.UpdateComment(1, 1, updateDto).Result
        
        // Assert
        result |> should be instanceOfType<OkObjectResult>
        let okResult = result :?> OkObjectResult
        let comment = okResult.Value :?> CommentDto
        comment.Autor |> should equal "Testador Atualizado"
        comment.Texto |> should equal "Comentário atualizado"
        
        context.Dispose()
    
    [<Fact>]
    let ``DeleteComment deve deletar comentario com sucesso`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "DeleteCommentTest"
        let controller = PostsController(context)
        
        // Act
        let result = controller.DeleteComment(1, 1).Result
        
        // Assert
        result |> should be instanceOfType<NoContentResult>
        
        // Verificar se foi deletado
        let getResult = controller.GetCommentById(1, 1).Result
        getResult |> should be instanceOfType<NotFoundObjectResult>
        
        context.Dispose()
    
    [<Fact>]
    let ``DeletePost deve deletar comentarios em cascade`` () =
        // Arrange
        let context = TestDbContextFactory.createSeededContext "CascadeDeleteTest"
        let controller = PostsController(context)
        
        // Verificar que existem comentários antes
        let postBefore = controller.GetPostById(1).Result :?> OkObjectResult
        let postDetail = postBefore.Value :?> BlogPostDetailDto
        postDetail.Comentarios |> List.length |> should equal 2
        
        // Act - Deletar o post
        let deleteResult = controller.DeletePost(1).Result
        
        // Assert
        deleteResult |> should be instanceOfType<NoContentResult>
        
        // Verificar que o comentário também foi deletado
        let commentResult = controller.GetCommentById(1, 1).Result
        commentResult |> should be instanceOfType<NotFoundObjectResult>
        
        context.Dispose()
