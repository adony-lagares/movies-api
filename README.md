# 🎮 Movies API

Movies API é uma API RESTful projetada para gerenciar filmes favoritos.Ela se integra com a **OMDB API** para buscar detalhes de filmes e permite que os usuários autentiquem e gerenciem seus filmes favoritos com segurança.

---

## 🚀 Funcionalidades

- ✅ **Autenticação de Usuário**: Autenticação baseada em JWT
- 🎭 **Busca de Filmes**: Obtém filmes da OMDB API
- ⭐ **Gerenciamento de Favoritos**: Adiciona, remove e lista filmes favoritos
- 📊 **Registro de Logs**: Logs estruturados com Serilog armazenados no Elasticsearch
- 🛡 **Rate Limiting**: Proteção contra requisições excessivas
- ❤️ **Health Checks**: Monitora o status da API
- 📝 **Swagger UI**: Documentação interativa da API
- 🤮 **Testes Automatizados**: Testes unitários e de integração com xUnit e Moq

---

## 🛠️ Tecnologias Utilizadas

- **ASP.NET Core 8** - Framework Web API
- **Entity Framework Core** - ORM para gerenciamento do banco de dados
- **SQL Server Express** - Banco de dados
- **Docker & Docker Compose** - Ambiente containerizado
- **Serilog + Elasticsearch** - Logs estruturados
- **Autenticação JWT** - Autenticação segura de usuários
- **Swagger UI** - Documentação da API
- **xUnit e Moq** - Frameworks para testes automatizados
- **OMDB API** - Integração para obter detalhes de filmes

---

## 💂️ Estrutura do Projeto

```
📦 MoviesApiSolution
 ├ 📂 MoviesApi
 ┃ ├ 📂 Controllers         # Endpoints da API
 ┃ ├ 📂 Data               # Configuração do Banco de Dados
 ┃ ├ 📂 DTOs               # Objetos de Transferência de Dados
 ┃ ├ 📂 Entities           # Entidades do Banco de Dados
 ┃ ├ 📂 Migrations         # Migrações do Entity Framework
 ┃ ├ 📂 Repositories       # Camada de Acesso a Dados
 ┃ ├ 📂 Services           # Camada de Lógica de Negócio
 ┃ ├ 📜 appsettings.example.json   # Modelo de configuração principal da API
 ┃ ├ 📜 Dockerfile         # Arquivo de configuração do Docker
 ┃ ├ 📜 MoviesApi.csproj   # Arquivo de configuração do projeto .NET
 ┃ ├ 📜 Program.cs         # Ponto de Entrada da Aplicação
 ├ 📂 MoviesApi.Tests
 ┃ ├ 📜 MoviesApi.Tests.csproj  # Configuração do projeto de testes
 ┃ ├ 📜 UserServiceTests.cs    # Testes do serviço de usuários
 ├ 📜 .gitignore          # Arquivo para ignorar arquivos desnecessários no Git
 ├ 📜 docker-compose.yml  # Configuração do Docker Compose
 ├ 📜 MoviesApiSolution.sln  # Arquivo da solução .NET
```

---

## 🔧 Configuração da OMDB API

A Movies API utiliza a **OMDB API** para buscar detalhes de filmes. Para utilizá-la, siga os passos abaixo:

1. Acesse o site oficial: [OMDB API](https://www.omdbapi.com/)
2. Clique em **API Key** e registre-se para obter uma chave de API gratuita.
3. No arquivo `appsettings.json`, substitua `SUA_CHAVE_OMDB_AQUI` pela sua API Key.

Assim a API poderá buscar detalhes de filmes diretamente da OMDB API.

---

## 🔧 Instalação e Configuração

### 1️⃣ **Pré-requisitos**

- **.NET SDK 8+** → [Baixar Aqui](https://dotnet.microsoft.com/download)
- **Docker & Docker Compose** → [Baixar Aqui](https://www.docker.com/)
- **SQL Server Express (LocalDB)** → [Baixar Aqui](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)

### 2️⃣ **Configuração das Variáveis de Ambiente**

1. Copie o arquivo de exemplo:
   ```bash
   cp appsettings.example.json appsettings.json
   ```
2. Edite o arquivo `appsettings.json` com suas credenciais reais.

---

## 🔧 Executando o Projeto Localmente

### 1️⃣ Instalando Dependências
   ```bash
  dotnet restore
   ```
### 2️⃣ Aplicando Migrações do Banco de Dados
   ```bash
dotnet ef database update
   ```
(Em caso de erro ' No executable found matching command "dotnet-ef" ') executar comando: dotnet tool install --global dotnet-ef e tentar novamente.

### 3️⃣ Executando a API
   ```bash
dotnet run
   ```
---

## 🔧 Executando o Projeto com Docker

### 1️⃣ Construindo e Executando os Containers
   ```bash
docker-compose up --build
   ```

Isso iniciará a API, o banco de dados e os serviços adicionais.

### 2️⃣ Acessando os Serviços
- Base URL da API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger

### 3️⃣ Parando os Containers
   ```bash
docker-compose down
   ```
### 4️⃣ Visualizando Logs em Tempo Real
   ```bash
docker-compose logs -f
   ```
---

## 📊 Monitoramento de saúde da API.

Realizar o Health Check:

```
http://localhost:5000/health
```

Permite verificar o status da API.

---

## 🚢 Executando Testes Automatizados

### **Rodando todos os testes**

```bash
dotnet test
```
---
