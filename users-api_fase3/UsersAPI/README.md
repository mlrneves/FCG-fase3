# FCG Game Store API (.NET 8)

API desenvolvida como parte do desafio da pós-graduação, simulando uma **loja de jogos** com:

- Cadastro e autenticação de usuários
- Perfis **User** e **Admin**
- Cadastro e gerenciamento de jogos
- Compras de jogos (biblioteca do usuário)
- Cadastro de promoções (Admin)
- Autenticação via **JWT**
- Logs estruturados e middleware de tratamento de erros

---

## Arquitetura do Projeto

Solução dividida em camadas:

- **Core**  
  - Entidades (`User`, `Game`, `Purchase`, `Promotion`)  
  - DTOs (`UserInput`, `UserUpdateInput`, `GameInput`, `GameUpdateInput`, `PurchaseInput`, `PromotionInput`, `LoginInput`, etc.)  
  - Interfaces de repositórios e serviços

- **Infrastructure**  
  - `ApplicationDbContext` (EF Core)  
  - Configurações de mapeamento (`*Configuration`)  
  - Repositórios concretos (`UserRepository`, `GameRepository`, `PurchaseRepository`, `PromotionRepository`)  
  - Serviços (`UserService`, `AuthService`, `GameService`, `PurchaseService`, `PromotionService`)

- **FCGApi**  
  - Projeto Web API (.NET 8)  
  - `Program.cs` (injeção de dependência, JWT, Swagger, middleware)  
  - Controllers (`UsersController`, `AuthController`, `GamesController`, `PurchasesController`, `PromotionsController`)  
  - Middleware de logs/erros

---

## Requisitos

- **.NET 8 SDK**
- **SQL Server** (ou LocalDB), por exemplo:
  - `(localdb)\\MSSQLLocalDB`
- Ferramenta de desenvolvimento:
  - Visual Studio 2022 **ou**
  - VS Code + CLI `dotnet`

---

## Configuração do Banco de Dados

A connection string está no arquivo `FCGApi/appsettings.json`:

```json
"ConnectionStrings": {
  "ConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=FCG;Trusted_Connection=True;TrustServerCertificate=True"
}

## Migrations e criação do banco de dados

O projeto usa **Entity Framework Core** com o `ApplicationDbContext` no projeto **Infrastructure**.

As migrations ficam em:

```text
Infrastructure/Repository/Migrations

## Como executar o `Update-Database` (criar o banco)

### Usando o Package Manager Console no Visual Studio

Se você nunca rodou o `Update-Database` antes, siga o passo a passo:

1. Abra a solução `FCG.sln` no Visual Studio.
2. Na Solution Explorer, clique com o botão direito no projeto **FCGApi** e selecione  
   **Set as Startup Project** (Definir como projeto de inicialização).
3. No menu superior, vá em:  
   **Tools → NuGet Package Manager → Package Manager Console**.
4. Vai abrir uma janela chamada **Package Manager Console** (geralmente na parte de baixo do VS).
5. No canto direito dessa janela, tem o combo **Default project**. Selecione **Infrastructure**.
6. No prompt da janela, digite o comando abaixo e pressione **Enter**:

   ```powershell
   Update-Database -Project Infrastructure -StartupProject FCGApi
