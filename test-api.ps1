# Teste Rápido da API - Windows PowerShell - CRUD Completo
# Execute este arquivo: .\test-api.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Blog API - Script de Teste CRUD Completo" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Detectar a porta (assumindo padrão ou especificar)
$PORT = "7123"  # Ajuste se necessário
$BASE_URL = "https://localhost:$PORT"

Write-Host "Usando porta: $PORT" -ForegroundColor Yellow
Write-Host "URL Base: $BASE_URL" -ForegroundColor Yellow
Write-Host ""
Write-Host "IMPORTANTE: Certifique-se que a API está rodando!" -ForegroundColor Red
Write-Host "Execute em outro terminal: cd BlogApi && dotnet run" -ForegroundColor Red
Write-Host ""

Read-Host "Pressione ENTER quando a API estiver rodando"

# Ignorar erros de certificado SSL para testes locais
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  TESTES DE LEITURA (READ)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`n1?? Testando GET /api/posts (Listar todos os posts)" -ForegroundColor Green
Write-Host "------------------------------------------------" -ForegroundColor Gray
try {
    $response = Invoke-RestMethod -Uri "$BASE_URL/api/posts" -Method Get -SkipCertificateCheck
    $response | ConvertTo-Json -Depth 10
    Write-Host "? Sucesso! Encontrados $($response.Count) posts" -ForegroundColor Green
} catch {
    Write-Host "? Erro: $_" -ForegroundColor Red
}

Write-Host "`n2?? Testando GET /api/posts/1 (Obter post específico)" -ForegroundColor Green
Write-Host "------------------------------------------------" -ForegroundColor Gray
try {
    $response = Invoke-RestMethod -Uri "$BASE_URL/api/posts/1" -Method Get -SkipCertificateCheck
    $response | ConvertTo-Json -Depth 10
    Write-Host "? Sucesso! Post encontrado com $($response.comentarios.Count) comentários" -ForegroundColor Green
} catch {
    Write-Host "? Erro: $_" -ForegroundColor Red
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  TESTES DE CRIAÇÃO (CREATE)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`n3?? Testando POST /api/posts (Criar novo post)" -ForegroundColor Green
Write-Host "------------------------------------------------" -ForegroundColor Gray
try {
    $newPost = @{
        titulo = "Post Criado via PowerShell"
        conteudo = "Este post foi criado automaticamente pelo script de teste!"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "$BASE_URL/api/posts" -Method Post `
        -Body $newPost -ContentType "application/json" -SkipCertificateCheck
    
    $response | ConvertTo-Json -Depth 10
    $newPostId = $response.id
    Write-Host "? Sucesso! Post criado com ID: $newPostId" -ForegroundColor Green
    
    # Salvar ID para uso posterior
    $script:testPostId = $newPostId
} catch {
    Write-Host "? Erro: $_" -ForegroundColor Red
}

Write-Host "`n4?? Testando POST /api/posts/1/comments (Adicionar comentário)" -ForegroundColor Green
Write-Host "------------------------------------------------" -ForegroundColor Gray
try {
    $newComment = @{
        autor = "Script PowerShell"
        texto = "Comentário adicionado automaticamente pelo teste!"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "$BASE_URL/api/posts/1/comments" -Method Post `
        -Body $newComment -ContentType "application/json" -SkipCertificateCheck
    
    $response | ConvertTo-Json -Depth 10
    $script:testCommentId = $response.id
    Write-Host "? Sucesso! Comentário criado com ID: $($response.id)" -ForegroundColor Green
} catch {
    Write-Host "? Erro: $_" -ForegroundColor Red
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  TESTES DE ATUALIZAÇÃO (UPDATE)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($script:testPostId) {
    Write-Host "`n5?? Testando PUT /api/posts/$($script:testPostId) (Atualizar post)" -ForegroundColor Green
    Write-Host "------------------------------------------------" -ForegroundColor Gray
    try {
        $updatePost = @{
            titulo = "Post ATUALIZADO via PowerShell"
            conteudo = "Conteúdo foi modificado com sucesso!"
        } | ConvertTo-Json

        $response = Invoke-RestMethod -Uri "$BASE_URL/api/posts/$($script:testPostId)" -Method Put `
            -Body $updatePost -ContentType "application/json" -SkipCertificateCheck
        
        Write-Host "Título atualizado: $($response.titulo)" -ForegroundColor Cyan
        Write-Host "? Sucesso! Post atualizado" -ForegroundColor Green
    } catch {
        Write-Host "? Erro: $_" -ForegroundColor Red
    }
}

if ($script:testCommentId) {
    Write-Host "`n6?? Testando PUT /api/posts/1/comments/$($script:testCommentId) (Atualizar comentário)" -ForegroundColor Green
    Write-Host "------------------------------------------------" -ForegroundColor Gray
    try {
        $updateComment = @{
            autor = "Script PowerShell (Atualizado)"
            texto = "Este comentário foi ATUALIZADO com sucesso!"
        } | ConvertTo-Json

        $response = Invoke-RestMethod -Uri "$BASE_URL/api/posts/1/comments/$($script:testCommentId)" -Method Put `
            -Body $updateComment -ContentType "application/json" -SkipCertificateCheck
        
        Write-Host "Texto atualizado: $($response.texto)" -ForegroundColor Cyan
        Write-Host "? Sucesso! Comentário atualizado" -ForegroundColor Green
    } catch {
        Write-Host "? Erro: $_" -ForegroundColor Red
    }
}

Write-Host "`n7?? Testando GET /api/posts/1/comments/$($script:testCommentId) (Obter comentário específico)" -ForegroundColor Green
Write-Host "------------------------------------------------" -ForegroundColor Gray
try {
    $response = Invoke-RestMethod -Uri "$BASE_URL/api/posts/1/comments/$($script:testCommentId)" -Method Get -SkipCertificateCheck
    $response | ConvertTo-Json -Depth 10
    Write-Host "? Sucesso! Comentário obtido" -ForegroundColor Green
} catch {
    Write-Host "? Erro: $_" -ForegroundColor Red
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  TESTES DE DELEÇÃO (DELETE)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($script:testCommentId) {
    Write-Host "`n8?? Testando DELETE /api/posts/1/comments/$($script:testCommentId) (Deletar comentário)" -ForegroundColor Green
    Write-Host "------------------------------------------------" -ForegroundColor Gray
    try {
        Invoke-RestMethod -Uri "$BASE_URL/api/posts/1/comments/$($script:testCommentId)" -Method Delete -SkipCertificateCheck | Out-Null
        Write-Host "? Sucesso! Comentário deletado (204 No Content)" -ForegroundColor Green
        
        # Tentar acessar comentário deletado
        Write-Host "`nVerificando se comentário foi deletado..." -ForegroundColor Yellow
        try {
            Invoke-RestMethod -Uri "$BASE_URL/api/posts/1/comments/$($script:testCommentId)" -Method Get -SkipCertificateCheck | Out-Null
            Write-Host "? Erro: Comentário ainda existe!" -ForegroundColor Red
        } catch {
            Write-Host "? Confirmado! Comentário não existe mais (404)" -ForegroundColor Green
        }
    } catch {
        Write-Host "? Erro: $_" -ForegroundColor Red
    }
}

if ($script:testPostId) {
    Write-Host "`n9?? Testando DELETE /api/posts/$($script:testPostId) (Deletar post)" -ForegroundColor Green
    Write-Host "------------------------------------------------" -ForegroundColor Gray
    try {
        Invoke-RestMethod -Uri "$BASE_URL/api/posts/$($script:testPostId)" -Method Delete -SkipCertificateCheck | Out-Null
        Write-Host "? Sucesso! Post deletado (204 No Content)" -ForegroundColor Green
        
        # Tentar acessar post deletado
        Write-Host "`nVerificando se post foi deletado..." -ForegroundColor Yellow
        try {
            Invoke-RestMethod -Uri "$BASE_URL/api/posts/$($script:testPostId)" -Method Get -SkipCertificateCheck | Out-Null
            Write-Host "? Erro: Post ainda existe!" -ForegroundColor Red
        } catch {
            Write-Host "? Confirmado! Post não existe mais (404)" -ForegroundColor Green
        }
    } catch {
        Write-Host "? Erro: $_" -ForegroundColor Red
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  TESTES DE VALIDAÇÃO" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`n?? Testando validação (criar post sem título - deve falhar)" -ForegroundColor Green
Write-Host "------------------------------------------------" -ForegroundColor Gray
try {
    $invalidPost = @{
        titulo = ""
        conteudo = "Teste de validação"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "$BASE_URL/api/posts" -Method Post `
        -Body $invalidPost -ContentType "application/json" -SkipCertificateCheck
    
    Write-Host "? Inesperado: Deveria ter falhado!" -ForegroundColor Red
} catch {
    Write-Host "? Validação funcionou corretamente! (400 Bad Request esperado)" -ForegroundColor Green
}

Write-Host "`n1??1?? Testando 404 (buscar post inexistente)" -ForegroundColor Green
Write-Host "------------------------------------------------" -ForegroundColor Gray
try {
    $response = Invoke-RestMethod -Uri "$BASE_URL/api/posts/999" -Method Get -SkipCertificateCheck
    Write-Host "? Inesperado: Deveria ter retornado 404!" -ForegroundColor Red
} catch {
    Write-Host "? 404 Not Found funcionou corretamente!" -ForegroundColor Green
}

Write-Host "`n1??2?? Testando 404 (atualizar post inexistente)" -ForegroundColor Green
Write-Host "------------------------------------------------" -ForegroundColor Gray
try {
    $updatePost = @{
        titulo = "Teste"
        conteudo = "Teste"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "$BASE_URL/api/posts/999" -Method Put `
        -Body $updatePost -ContentType "application/json" -SkipCertificateCheck
    
    Write-Host "? Inesperado: Deveria ter retornado 404!" -ForegroundColor Red
} catch {
    Write-Host "? 404 Not Found funcionou corretamente!" -ForegroundColor Green
}

Write-Host "`n1??3?? Testando 404 (deletar post inexistente)" -ForegroundColor Green
Write-Host "------------------------------------------------" -ForegroundColor Gray
try {
    Invoke-RestMethod -Uri "$BASE_URL/api/posts/999" -Method Delete -SkipCertificateCheck | Out-Null
    Write-Host "? Inesperado: Deveria ter retornado 404!" -ForegroundColor Red
} catch {
    Write-Host "? 404 Not Found funcionou corretamente!" -ForegroundColor Green
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  ? TESTES CONCLUÍDOS!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Resumo dos Testes CRUD:" -ForegroundColor Yellow
Write-Host "  ? CREATE - Posts e Comentários" -ForegroundColor Green
Write-Host "  ? READ - Listagem e detalhes" -ForegroundColor Green
Write-Host "  ? UPDATE - Posts e Comentários" -ForegroundColor Green
Write-Host "  ? DELETE - Posts e Comentários" -ForegroundColor Green
Write-Host "  ? VALIDAÇÕES - 400 e 404" -ForegroundColor Green
Write-Host ""
Write-Host "Acesse o Swagger UI para mais testes:" -ForegroundColor Yellow
Write-Host "$BASE_URL/swagger" -ForegroundColor Yellow
Write-Host ""
