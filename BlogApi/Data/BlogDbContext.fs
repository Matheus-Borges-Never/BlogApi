namespace BlogApi.Data

open System
open System.Linq.Expressions
open Microsoft.EntityFrameworkCore
open BlogApi.Models

type BlogDbContext(options: DbContextOptions<BlogDbContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable private blogPosts: DbSet<BlogPost>
    
    [<DefaultValue>]
    val mutable private comments: DbSet<Comment>

    member this.BlogPosts 
        with get() = this.blogPosts
        and set value = this.blogPosts <- value

    member this.Comments 
        with get() = this.comments
        and set value = this.comments <- value

    override this.OnModelCreating(modelBuilder: ModelBuilder) =
        base.OnModelCreating(modelBuilder)
        
        modelBuilder.Entity<BlogPost>()
            .HasMany(fun p -> p.Comments :> seq<Comment>)
            .WithOne(fun c -> c.BlogPost)
            .HasForeignKey(fun c -> c.BlogPostId :> obj)
            .OnDelete(DeleteBehavior.Cascade) |> ignore

        modelBuilder.Entity<BlogPost>().HasData(
            {
                Id = 1
                Titulo = "Bem-vindo ao Blog API"
                Conteudo = "Este é o primeiro post do blog. A API está funcionando perfeitamente!"
                DataCriacao = DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc)
                Comments = Unchecked.defaultof<_>
            }
        ) |> ignore

        modelBuilder.Entity<Comment>().HasData(
            {
                Id = 1
                Autor = "João Silva"
                Texto = "Ótimo post! Muito informativo."
                DataCriacao = DateTime(2025, 1, 15, 11, 0, 0, DateTimeKind.Utc)
                BlogPostId = 1
                BlogPost = Unchecked.defaultof<_>
            },
            {
                Id = 2
                Autor = "Maria Santos"
                Texto = "Adorei a explicação!"
                DataCriacao = DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc)
                BlogPostId = 1
                BlogPost = Unchecked.defaultof<_>
            }
        ) |> ignore
