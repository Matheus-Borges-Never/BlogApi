namespace BlogApi.Tests.Helpers

open System
open Microsoft.EntityFrameworkCore
open BlogApi.Data
open BlogApi.Models

module TestDbContextFactory =
    
    let createInMemoryContext (databaseName: string) =
        let options = 
            DbContextOptionsBuilder<BlogDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options
        
        new BlogDbContext(options)
    
    let seedTestData (context: BlogDbContext) =
        let post1 = {
            Id = 1
            Titulo = "Post de Teste 1"
            Conteudo = "Conteúdo do post de teste 1"
            DataCriacao = DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            Comments = System.Collections.Generic.List<Comment>() :> System.Collections.Generic.ICollection<Comment>
        }
        
        let post2 = {
            Id = 2
            Titulo = "Post de Teste 2"
            Conteudo = "Conteúdo do post de teste 2"
            DataCriacao = DateTime(2024, 1, 16, 10, 0, 0, DateTimeKind.Utc)
            Comments = System.Collections.Generic.List<Comment>() :> System.Collections.Generic.ICollection<Comment>
        }
        
        context.BlogPosts.Add(post1) |> ignore
        context.BlogPosts.Add(post2) |> ignore
        context.SaveChanges() |> ignore
        
        let comment1 = {
            Id = 1
            Autor = "Testador 1"
            Texto = "Comentário de teste 1"
            DataCriacao = DateTime(2024, 1, 15, 11, 0, 0, DateTimeKind.Utc)
            BlogPostId = 1
            BlogPost = Unchecked.defaultof<_>
        }
        
        let comment2 = {
            Id = 2
            Autor = "Testador 2"
            Texto = "Comentário de teste 2"
            DataCriacao = DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc)
            BlogPostId = 1
            BlogPost = Unchecked.defaultof<_>
        }
        
        context.Comments.Add(comment1) |> ignore
        context.Comments.Add(comment2) |> ignore
        context.SaveChanges() |> ignore
        
        context
    
    let createSeededContext (databaseName: string) =
        let context = createInMemoryContext databaseName
        seedTestData context
