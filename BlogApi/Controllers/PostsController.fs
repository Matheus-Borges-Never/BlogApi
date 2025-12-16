namespace BlogApi.Controllers

open System
open System.Linq
open Microsoft.AspNetCore.Mvc
open Microsoft.EntityFrameworkCore
open BlogApi.Data
open BlogApi.Models
open BlogApi.DTOs

[<ApiController>]
[<Route("api/[controller]")>]
type PostsController(context: BlogDbContext) =
    inherit ControllerBase()

    // GET /api/posts
    [<HttpGet>]
    member this.GetAllPosts() =
        async {
            let! posts = 
                context.BlogPosts
                    .Include(fun p -> p.Comments)
                    .ToListAsync()
                |> Async.AwaitTask

            let postsList = 
                posts 
                |> Seq.map (fun p -> 
                    {
                        Id = p.Id
                        Titulo = p.Titulo
                        DataCriacao = p.DataCriacao
                        NumeroComentarios = p.Comments.Count
                    })
                |> Seq.toList

            return this.Ok(postsList) :> IActionResult
        } |> Async.StartAsTask

    // POST /api/posts
    [<HttpPost>]
    member this.CreatePost([<FromBody>] createDto: CreateBlogPostDto) =
        async {
            if not this.ModelState.IsValid then
                return this.BadRequest(this.ModelState) :> IActionResult
            else
                let newPost = {
                    Id = 0
                    Titulo = createDto.Titulo
                    Conteudo = createDto.Conteudo
                    DataCriacao = DateTime.UtcNow
                    Comments = System.Collections.Generic.List<Comment>() :> System.Collections.Generic.ICollection<Comment>
                }

                context.BlogPosts.Add(newPost) |> ignore
                let! _ = context.SaveChangesAsync() |> Async.AwaitTask

                let detailDto = {
                    Id = newPost.Id
                    Titulo = newPost.Titulo
                    Conteudo = newPost.Conteudo
                    DataCriacao = newPost.DataCriacao
                    Comentarios = []
                }

                return this.CreatedAtAction("GetPostById", {| id = newPost.Id |}, detailDto) :> IActionResult
        } |> Async.StartAsTask

    // GET /api/posts/{id}
    [<HttpGet("{id}")>]
    member this.GetPostById(id: int) =
        async {
            let! post = 
                context.BlogPosts
                    .Include(fun p -> p.Comments)
                    .FirstOrDefaultAsync(fun p -> p.Id = id)
                |> Async.AwaitTask

            if isNull (box post) then
                return this.NotFound({| message = sprintf "Post com ID %d não encontrado" id |}) :> IActionResult
            else
                let comentarios = 
                    post.Comments
                    |> Seq.map (fun c -> 
                        {
                            Id = c.Id
                            Autor = c.Autor
                            Texto = c.Texto
                            DataCriacao = c.DataCriacao
                        })
                    |> Seq.sortByDescending (fun c -> c.DataCriacao)
                    |> Seq.toList

                let detailDto = {
                    Id = post.Id
                    Titulo = post.Titulo
                    Conteudo = post.Conteudo
                    DataCriacao = post.DataCriacao
                    Comentarios = comentarios
                }

                return this.Ok(detailDto) :> IActionResult
        } |> Async.StartAsTask

    // PUT /api/posts/{id}
    [<HttpPut("{id}")>]
    member this.UpdatePost(id: int, [<FromBody>] updateDto: UpdateBlogPostDto) =
        async {
            if not this.ModelState.IsValid then
                return this.BadRequest(this.ModelState) :> IActionResult
            else
                let! post = 
                    context.BlogPosts
                        .FirstOrDefaultAsync(fun p -> p.Id = id)
                    |> Async.AwaitTask

                if isNull (box post) then
                    return this.NotFound({| message = sprintf "Post com ID %d não encontrado" id |}) :> IActionResult
                else
                    let updatedPost = {
                        post with
                            Titulo = updateDto.Titulo
                            Conteudo = updateDto.Conteudo
                    }

                    context.Entry(post).CurrentValues.SetValues(updatedPost) |> ignore
                    let! _ = context.SaveChangesAsync() |> Async.AwaitTask

                    let! updatedPostWithComments = 
                        context.BlogPosts
                            .Include(fun p -> p.Comments)
                            .FirstOrDefaultAsync(fun p -> p.Id = id)
                        |> Async.AwaitTask

                    let comentarios = 
                        updatedPostWithComments.Comments
                        |> Seq.map (fun c -> 
                            {
                                Id = c.Id
                                Autor = c.Autor
                                Texto = c.Texto
                                DataCriacao = c.DataCriacao
                            })
                        |> Seq.sortByDescending (fun c -> c.DataCriacao)
                        |> Seq.toList

                    let detailDto = {
                        Id = updatedPostWithComments.Id
                        Titulo = updatedPostWithComments.Titulo
                        Conteudo = updatedPostWithComments.Conteudo
                        DataCriacao = updatedPostWithComments.DataCriacao
                        Comentarios = comentarios
                    }

                    return this.Ok(detailDto) :> IActionResult
        } |> Async.StartAsTask

    // DELETE /api/posts/{id}
    [<HttpDelete("{id}")>]
    member this.DeletePost(id: int) =
        async {
            let! post = 
                context.BlogPosts
                    .FirstOrDefaultAsync(fun p -> p.Id = id)
                |> Async.AwaitTask

            if isNull (box post) then
                return this.NotFound({| message = sprintf "Post com ID %d não encontrado" id |}) :> IActionResult
            else
                context.BlogPosts.Remove(post) |> ignore
                let! _ = context.SaveChangesAsync() |> Async.AwaitTask

                return this.NoContent() :> IActionResult
        } |> Async.StartAsTask

    // POST /api/posts/{id}/comments
    [<HttpPost("{id}/comments")>]
    member this.AddComment(id: int, [<FromBody>] createCommentDto: CreateCommentDto) =
        async {
            if not this.ModelState.IsValid then
                return this.BadRequest(this.ModelState) :> IActionResult
            else
                let! post = 
                    context.BlogPosts
                        .FirstOrDefaultAsync(fun p -> p.Id = id)
                    |> Async.AwaitTask

                if isNull (box post) then
                    return this.NotFound({| message = sprintf "Post com ID %d não encontrado" id |}) :> IActionResult
                else
                    let newComment = {
                        Id = 0
                        Autor = createCommentDto.Autor
                        Texto = createCommentDto.Texto
                        DataCriacao = DateTime.UtcNow
                        BlogPostId = id
                        BlogPost = Unchecked.defaultof<_>
                    }

                    context.Comments.Add(newComment) |> ignore
                    let! _ = context.SaveChangesAsync() |> Async.AwaitTask

                    let commentDto = {
                        Id = newComment.Id
                        Autor = newComment.Autor
                        Texto = newComment.Texto
                        DataCriacao = newComment.DataCriacao
                    }

                    return this.Created(sprintf "/api/posts/%d/comments/%d" id newComment.Id, commentDto) :> IActionResult
        } |> Async.StartAsTask

    // GET /api/posts/{postId}/comments/{commentId}
    [<HttpGet("{postId}/comments/{commentId}")>]
    member this.GetCommentById(postId: int, commentId: int) =
        async {
            let! comment = 
                context.Comments
                    .FirstOrDefaultAsync(fun c -> c.Id = commentId && c.BlogPostId = postId)
                |> Async.AwaitTask

            if isNull (box comment) then
                return this.NotFound({| message = sprintf "Comentário com ID %d não encontrado no post %d" commentId postId |}) :> IActionResult
            else
                let commentDto = {
                    Id = comment.Id
                    Autor = comment.Autor
                    Texto = comment.Texto
                    DataCriacao = comment.DataCriacao
                }

                return this.Ok(commentDto) :> IActionResult
        } |> Async.StartAsTask

    // PUT /api/posts/{postId}/comments/{commentId}
    [<HttpPut("{postId}/comments/{commentId}")>]
    member this.UpdateComment(postId: int, commentId: int, [<FromBody>] updateDto: UpdateCommentDto) =
        async {
            if not this.ModelState.IsValid then
                return this.BadRequest(this.ModelState) :> IActionResult
            else
                let! comment = 
                    context.Comments
                        .FirstOrDefaultAsync(fun c -> c.Id = commentId && c.BlogPostId = postId)
                    |> Async.AwaitTask

                if isNull (box comment) then
                    return this.NotFound({| message = sprintf "Comentário com ID %d não encontrado no post %d" commentId postId |}) :> IActionResult
                else
                    let updatedComment = {
                        comment with
                            Autor = updateDto.Autor
                            Texto = updateDto.Texto
                    }

                    context.Entry(comment).CurrentValues.SetValues(updatedComment) |> ignore
                    let! _ = context.SaveChangesAsync() |> Async.AwaitTask

                    let commentDto = {
                        Id = updatedComment.Id
                        Autor = updatedComment.Autor
                        Texto = updatedComment.Texto
                        DataCriacao = updatedComment.DataCriacao
                    }

                    return this.Ok(commentDto) :> IActionResult
        } |> Async.StartAsTask

    // DELETE /api/posts/{postId}/comments/{commentId}
    [<HttpDelete("{postId}/comments/{commentId}")>]
    member this.DeleteComment(postId: int, commentId: int) =
        async {
            let! comment = 
                context.Comments
                    .FirstOrDefaultAsync(fun c -> c.Id = commentId && c.BlogPostId = postId)
                |> Async.AwaitTask

            if isNull (box comment) then
                return this.NotFound({| message = sprintf "Comentário com ID %d não encontrado no post %d" commentId postId |}) :> IActionResult
            else
                context.Comments.Remove(comment) |> ignore
                let! _ = context.SaveChangesAsync() |> Async.AwaitTask

                return this.NoContent() :> IActionResult
        } |> Async.StartAsTask
