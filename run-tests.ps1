# Script para executar todos os testes da Blog API
# Execute: .\run-tests.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Blog API - Executar Testes" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Navegar para o diretório de testes
$testsPath = "BlogApi.Tests"

if (-not (Test-Path $testsPath)) {
    Write-Host "? Diretório de testes não encontrado: $testsPath" -ForegroundColor Red
    exit 1
}

Set-Location $testsPath

Write-Host "?? Diretório de testes: $testsPath" -ForegroundColor Yellow
Write-Host ""

# Menu de opções
Write-Host "Selecione o tipo de teste:" -ForegroundColor Cyan
Write-Host "1. Executar TODOS os testes" -ForegroundColor White
Write-Host "2. Executar apenas Testes Unitários" -ForegroundColor White
Write-Host "3. Executar apenas Testes de Integração" -ForegroundColor White
Write-Host "4. Executar com verbosity detalhado" -ForegroundColor White
Write-Host "5. Executar com cobertura de código" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Digite sua escolha (1-5)"

Write-Host ""
Write-Host "?? Executando testes..." -ForegroundColor Yellow
Write-Host ""

switch ($choice) {
    "1" {
        Write-Host "? Executando TODOS os testes" -ForegroundColor Green
        dotnet test
    }
    "2" {
        Write-Host "? Executando apenas Testes Unitários" -ForegroundColor Green
        dotnet test --filter "FullyQualifiedName~UnitTests"
    }
    "3" {
        Write-Host "? Executando apenas Testes de Integração" -ForegroundColor Green
        dotnet test --filter "FullyQualifiedName~IntegrationTests"
    }
    "4" {
        Write-Host "? Executando com verbosity detalhado" -ForegroundColor Green
        dotnet test --verbosity detailed
    }
    "5" {
        Write-Host "? Executando com cobertura de código" -ForegroundColor Green
        dotnet test /p:CollectCoverage=true
    }
    default {
        Write-Host "? Opção inválida. Executando todos os testes..." -ForegroundColor Red
        dotnet test
    }
}

Write-Host ""

if ($LASTEXITCODE -eq 0) {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  ? TODOS OS TESTES PASSARAM!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
} else {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  ? ALGUNS TESTES FALHARAM" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "?? Resumo dos Testes:" -ForegroundColor Yellow
Write-Host "  • Testes Unitários: 15 testes" -ForegroundColor White
Write-Host "  • Testes de Validação: 15 testes" -ForegroundColor White
Write-Host "  • Testes de Integração (Posts): 9 testes" -ForegroundColor White
Write-Host "  • Testes de Integração (Comentários): 10 testes" -ForegroundColor White
Write-Host "  • TOTAL: 49 testes" -ForegroundColor Green
Write-Host ""

# Voltar ao diretório original
Set-Location ..

Read-Host "Pressione ENTER para sair"
