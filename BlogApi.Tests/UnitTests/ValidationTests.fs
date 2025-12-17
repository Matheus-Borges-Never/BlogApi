namespace BlogApi.Tests.UnitTests

open System.ComponentModel.DataAnnotations
open Xunit
open FsUnit.Xunit
open BlogApi.DTOs

module ValidationTests =
    
    let validateModel<'T> (model: 'T) =
        let context = ValidationContext(model)
        let results = System.Collections.Generic.List<ValidationResult>()
        let isValid = Validator.TryValidateObject(model, context, results, true)
        (isValid, results)
    
    [<Fact>]
    let ``CreateBlogPostDto deve ser valido com dados corretos`` () =
        // Arrange
        let dto = {
            Titulo = "Post Válido"
            Conteudo = "Conteúdo válido do post"
        }
        
        // Act
        let (isValid, _) = validateModel dto
        
        // Assert
        isValid |> should equal true
    
    [<Fact>]
    let ``CreateBlogPostDto deve falhar com titulo vazio`` () =
        // Arrange
        let dto = {
            Titulo = ""
            Conteudo = "Conteúdo válido"
        }
        
        // Act
        let (isValid, errors) = validateModel dto
        
        // Assert
        isValid |> should equal false
        errors.Count |> should be (greaterThan 0)
    
    [<Fact>]
    let ``CreateBlogPostDto deve falhar com titulo muito longo`` () =
        // Arrange
        let dto = {
            Titulo = String.replicate 201 "a"
            Conteudo = "Conteúdo válido"
        }
        
        // Act
        let (isValid, errors) = validateModel dto
        
        // Assert
        isValid |> should equal false
        errors |> Seq.exists (fun e -> e.ErrorMessage.Contains("200")) |> should equal true
    
    [<Fact>]
    let ``CreateBlogPostDto deve falhar com conteudo vazio`` () =
        // Arrange
        let dto = {
            Titulo = "Título válido"
            Conteudo = ""
        }
        
        // Act
        let (isValid, errors) = validateModel dto
        
        // Assert
        isValid |> should equal false
        errors.Count |> should be (greaterThan 0)
    
    [<Fact>]
    let ``UpdateBlogPostDto deve ser valido com dados corretos`` () =
        // Arrange
        let dto = {
            Titulo = "Post Atualizado"
            Conteudo = "Conteúdo atualizado"
        }
        
        // Act
        let (isValid, _) = validateModel dto
        
        // Assert
        isValid |> should equal true
    
    [<Fact>]
    let ``CreateCommentDto deve ser valido com dados corretos`` () =
        // Arrange
        let dto = {
            Autor = "Autor Válido"
            Texto = "Comentário válido"
        }
        
        // Act
        let (isValid, _) = validateModel dto
        
        // Assert
        isValid |> should equal true
    
    [<Fact>]
    let ``CreateCommentDto deve falhar com autor vazio`` () =
        // Arrange
        let dto = {
            Autor = ""
            Texto = "Comentário válido"
        }
        
        // Act
        let (isValid, errors) = validateModel dto
        
        // Assert
        isValid |> should equal false
        errors.Count |> should be (greaterThan 0)
    
    [<Fact>]
    let ``CreateCommentDto deve falhar com autor muito longo`` () =
        // Arrange
        let dto = {
            Autor = String.replicate 101 "a"
            Texto = "Comentário válido"
        }
        
        // Act
        let (isValid, errors) = validateModel dto
        
        // Assert
        isValid |> should equal false
        errors |> Seq.exists (fun e -> e.ErrorMessage.Contains("100")) |> should equal true
    
    [<Fact>]
    let ``CreateCommentDto deve falhar com texto vazio`` () =
        // Arrange
        let dto = {
            Autor = "Autor válido"
            Texto = ""
        }
        
        // Act
        let (isValid, errors) = validateModel dto
        
        // Assert
        isValid |> should equal false
        errors.Count |> should be (greaterThan 0)
    
    [<Fact>]
    let ``CreateCommentDto deve falhar com texto muito longo`` () =
        // Arrange
        let dto = {
            Autor = "Autor válido"
            Texto = String.replicate 1001 "a"
        }
        
        // Act
        let (isValid, errors) = validateModel dto
        
        // Assert
        isValid |> should equal false
        errors |> Seq.exists (fun e -> e.ErrorMessage.Contains("1000")) |> should equal true
    
    [<Fact>]
    let ``UpdateCommentDto deve ser valido com dados corretos`` () =
        // Arrange
        let dto = {
            Autor = "Autor Atualizado"
            Texto = "Comentário atualizado"
        }
        
        // Act
        let (isValid, _) = validateModel dto
        
        // Assert
        isValid |> should equal true
    
    [<Fact>]
    let ``CreateBlogPostDto deve aceitar titulo no limite minimo`` () =
        // Arrange
        let dto = {
            Titulo = "A"
            Conteudo = "Conteúdo válido"
        }
        
        // Act
        let (isValid, _) = validateModel dto
        
        // Assert
        isValid |> should equal true
    
    [<Fact>]
    let ``CreateBlogPostDto deve aceitar titulo no limite maximo`` () =
        // Arrange
        let dto = {
            Titulo = String.replicate 200 "a"
            Conteudo = "Conteúdo válido"
        }
        
        // Act
        let (isValid, _) = validateModel dto
        
        // Assert
        isValid |> should equal true
    
    [<Fact>]
    let ``CreateCommentDto deve aceitar autor no limite minimo`` () =
        // Arrange
        let dto = {
            Autor = "A"
            Texto = "Comentário válido"
        }
        
        // Act
        let (isValid, _) = validateModel dto
        
        // Assert
        isValid |> should equal true
    
    [<Fact>]
    let ``CreateCommentDto deve aceitar autor no limite maximo`` () =
        // Arrange
        let dto = {
            Autor = String.replicate 100 "a"
            Texto = "Comentário válido"
        }
        
        // Act
        let (isValid, _) = validateModel dto
        
        // Assert
        isValid |> should equal true
