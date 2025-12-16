# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar arquivos do projeto
COPY ["BlogApi/BlogApi.fsproj", "BlogApi/"]
RUN dotnet restore "BlogApi/BlogApi.fsproj"

# Copiar todo o código
COPY . .
WORKDIR "/src/BlogApi"

# Build da aplicação
RUN dotnet build "BlogApi.fsproj" -c Release -o /app/build
RUN dotnet publish "BlogApi.fsproj" -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copiar arquivos publicados
COPY --from=build /app/publish .

# Expor porta
EXPOSE 8080

# Variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Executar aplicação
ENTRYPOINT ["dotnet", "BlogApi.dll"]
