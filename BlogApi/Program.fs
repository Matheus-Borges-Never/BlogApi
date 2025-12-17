namespace BlogApi
#nowarn "20"
open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.EntityFrameworkCore
open BlogApi.Data

type Program() =
    class end

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        // Configurar Entity Framework com SQLite
        builder.Services.AddDbContext<BlogDbContext>(fun options ->
            options.UseSqlite("Data Source=blogapi.db") |> ignore
        ) |> ignore

        // Adicionar controllers
        builder.Services.AddControllers() |> ignore

        // Adicionar Swagger para documentação da API
        builder.Services.AddEndpointsApiExplorer() |> ignore
        builder.Services.AddSwaggerGen() |> ignore

        let app = builder.Build()

        // Criar/atualizar banco de dados automaticamente
        use scope = app.Services.CreateScope()
        let services = scope.ServiceProvider
        let context = services.GetRequiredService<BlogDbContext>()
        context.Database.EnsureCreated() |> ignore

        // Configurar Swagger em desenvolvimento
        if app.Environment.IsDevelopment() then
            app.UseSwagger() |> ignore
            app.UseSwaggerUI() |> ignore

        app.UseHttpsRedirection() |> ignore

        app.UseAuthorization() |> ignore
        
        app.MapControllers() |> ignore

        app.Run()

        exitCode
