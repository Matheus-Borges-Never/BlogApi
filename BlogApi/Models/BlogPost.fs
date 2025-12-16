namespace BlogApi.Models

open System
open System.Collections.Generic
open System.ComponentModel.DataAnnotations

[<CLIMutable>]
type BlogPost = {
    [<Key>]
    Id: int
    Titulo: string
    Conteudo: string
    DataCriacao: DateTime
    Comments: ICollection<Comment>
}

and [<CLIMutable>] Comment = {
    [<Key>]
    Id: int
    Autor: string
    Texto: string
    DataCriacao: DateTime
    BlogPostId: int
    BlogPost: BlogPost
}
