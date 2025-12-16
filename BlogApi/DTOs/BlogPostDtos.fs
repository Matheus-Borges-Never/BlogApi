namespace BlogApi.DTOs

open System
open System.ComponentModel.DataAnnotations

[<CLIMutable>]
type CreateBlogPostDto = {
    [<Required>]
    [<StringLength(200, MinimumLength = 1)>]
    Titulo: string
    
    [<Required>]
    [<StringLength(10000, MinimumLength = 1)>]
    Conteudo: string
}

[<CLIMutable>]
type UpdateBlogPostDto = {
    [<Required>]
    [<StringLength(200, MinimumLength = 1)>]
    Titulo: string
    
    [<Required>]
    [<StringLength(10000, MinimumLength = 1)>]
    Conteudo: string
}

[<CLIMutable>]
type BlogPostListItemDto = {
    Id: int
    Titulo: string
    DataCriacao: DateTime
    NumeroComentarios: int
}

[<CLIMutable>]
type CommentDto = {
    Id: int
    Autor: string
    Texto: string
    DataCriacao: DateTime
}

[<CLIMutable>]
type BlogPostDetailDto = {
    Id: int
    Titulo: string
    Conteudo: string
    DataCriacao: DateTime
    Comentarios: CommentDto list
}

[<CLIMutable>]
type CreateCommentDto = {
    [<Required>]
    [<StringLength(100, MinimumLength = 1)>]
    Autor: string
    
    [<Required>]
    [<StringLength(1000, MinimumLength = 1)>]
    Texto: string
}

[<CLIMutable>]
type UpdateCommentDto = {
    [<Required>]
    [<StringLength(100, MinimumLength = 1)>]
    Autor: string
    
    [<Required>]
    [<StringLength(1000, MinimumLength = 1)>]
    Texto: string
}
