
# CatalogAPI

## Visão Geral

O **CatalogAPI** é o microsserviço responsável pelo **catálogo de jogos, promoções e início do fluxo de compra** no sistema FCG.

Ele centraliza as responsabilidades relacionadas a:

- cadastro, consulta, atualização e remoção de jogos
- cadastro, consulta, atualização e remoção de promoções
- criação de pedidos de compra
- publicação do evento `OrderPlacedEvent`
- consumo do evento `PaymentProcessedEvent`
- liberação do jogo na biblioteca do usuário quando o pagamento é aprovado

Este microsserviço faz parte de uma arquitetura baseada em **microserviços e comunicação assíncrona por eventos**, utilizando **RabbitMQ** e **MassTransit**.

---

# Responsabilidade do Microsserviço

O **CatalogAPI** possui as seguintes responsabilidades:

- gerenciar o catálogo de jogos
- gerenciar promoções aplicadas aos jogos
- iniciar o fluxo de compra
- calcular o preço final da compra, considerando promoções ativas
- registrar o pedido com status inicial `Pending`
- publicar `OrderPlacedEvent` para o `PaymentsAPI`
- consumir `PaymentProcessedEvent`
- atualizar o status da compra para `Approved` ou `Rejected`
- adicionar o jogo à biblioteca do usuário quando o pagamento for aprovado

Este serviço **não autentica usuários** e **não processa pagamentos**.  
A autenticação pertence ao **UsersAPI** e o processamento de pagamento pertence ao **PaymentsAPI**.

---

# Eventos

## Evento Publicado

### OrderPlacedEvent

Publicado quando o microsserviço recebe uma requisição de compra e registra o pedido.

Objetivo:

- iniciar o fluxo assíncrono de pagamento

Campos principais:

- `PurchaseId`
- `UserId`
- `GameId`
- `Price`

Consumidor esperado:

- `PaymentsAPI`

---

## Evento Consumido

### PaymentProcessedEvent

Consumido quando o `PaymentsAPI` finaliza o processamento do pagamento.

Objetivo:

- atualizar o status da compra
- adicionar o jogo à biblioteca do usuário quando o status for `Approved`

Campos principais:

- `PurchaseId`
- `UserId`
- `GameId`
- `Price`
- `Status`

Publisher esperado:

- `PaymentsAPI`

---

# Arquitetura

O projeto segue uma arquitetura em camadas.

## Core

Contém as regras de negócio e contratos da aplicação.

Responsabilidades:

- entidades do domínio
- interfaces de repositório
- interfaces de serviços
- contratos de eventos
- inputs e modelos de negócio

Objetivo:

- manter a lógica de negócio desacoplada de detalhes técnicos

---

## Infrastructure

Contém implementações técnicas e persistência.

Responsabilidades:

- implementação dos repositórios
- configuração do Entity Framework Core
- configuração do acesso ao banco
- implementação dos serviços
- consumers do MassTransit

Objetivo:

- isolar regras de persistência e integração do domínio

---

## CatalogAPI (Web API)

Camada responsável pela exposição da API HTTP.

Responsabilidades:

- controllers
- autenticação e autorização
- configuração do MassTransit
- configuração de DI
- Swagger
- middlewares
- inicialização da aplicação

Objetivo:

- expor endpoints REST para jogos, promoções, compras e biblioteca do usuário

---

# Tecnologias Utilizadas

- .NET 8
- ASP.NET Core
- Entity Framework Core
- SQL Server
- RabbitMQ
- MassTransit
- JWT Authentication
- Swagger
- Docker
- Kubernetes

---

# Requisitos

Para executar este microsserviço são necessários:

- .NET 8 SDK
- SQL Server ou SQL Server LocalDB
- RabbitMQ
- Docker (opcional)
- Kubernetes (para execução em cluster)

---

# Banco de Dados

Banco utilizado pelo serviço:

`FCGCatalog`

Principais tabelas do domínio:

- `Games`
- `Promotions`
- `Purchases`
- `UserLibraries`

As migrations são aplicadas automaticamente na inicialização da aplicação através de:

`Database.Migrate()`

---

# Regras Principais do Fluxo de Compra

1. O cliente chama o endpoint de compra no `CatalogAPI`
2. O serviço valida o jogo e calcula o preço final
3. O pedido é criado com status `Pending`
4. O `CatalogAPI` publica `OrderPlacedEvent`
5. O `PaymentsAPI` processa o pagamento
6. O `PaymentsAPI` publica `PaymentProcessedEvent`
7. O `CatalogAPI` consome o evento de retorno
8. Se o status for `Approved`, o jogo é adicionado à biblioteca do usuário

---

# Variáveis de Ambiente

## Banco

`ConnectionStrings__ConnectionString`

Exemplo:

`Server=sqlserver;Database=FCGCatalog;User Id=sa;Password=Your_password123;TrustServerCertificate=True`

---

## RabbitMQ

`RabbitMq__Host`  
`RabbitMq__Username`  
`RabbitMq__Password`

Exemplo:

`RabbitMq__Host=rabbitmq`  
`RabbitMq__Username=fcg`  
`RabbitMq__Password=fcg123`

---

## JWT

`Jwt__Issuer`  
`Jwt__Key`

Essas variáveis são necessárias porque o serviço expõe endpoints protegidos.

---

## Filas / Mensageria

`Queues__PaymentProcessed`

Exemplo:

`Queues__PaymentProcessed=catalog-payment-processed`

Observação: este microsserviço **consome** `PaymentProcessedEvent`, portanto faz sentido parametrizar o nome da fila de consumo.  
Ele **publica** `OrderPlacedEvent`, então não precisa de fila própria para publicação.

---

## URLs de Serviço

No estado atual do `CatalogAPI`, **não há chamada HTTP direta para outros microsserviços**, apenas comunicação por eventos.

Por esse motivo, **não é necessário** configurar URLs de serviços externos neste microsserviço neste momento.  
Se no futuro o `CatalogAPI` passar a consultar o `UsersAPI` ou outro serviço via HTTP, essas URLs devem ser externalizadas em `ConfigMap`.

---

# Execução Local

Comandos básicos:

`dotnet restore`  
`dotnet build`  
`dotnet run`

Swagger:

`http://localhost:{porta}/swagger`

---

# Docker

O microsserviço possui `Dockerfile` multi-stage, otimizado para produção.

Build da imagem:

`docker build -t fcg-catalog-api .`

Execução:

`docker run -p 5002:8080 fcg-catalog-api`

---

# Kubernetes

Manifestos disponíveis na pasta:

`/k8s`

Arquivos esperados:

- `catalog-api-configmap.yaml`
- `catalog-api-secret.yaml`
- `catalog-api-deployment.yaml`
- `catalog-api-service.yaml`

Esses manifestos garantem:

- gerenciamento dos pods via `Deployment`
- configuração não sensível via `ConfigMap`
- configuração sensível via `Secret`
- exposição interna do serviço via `Service`

---

# Comunicação Assíncrona

Fluxo principal:

`CatalogAPI → OrderPlacedEvent → PaymentsAPI`

Fluxo de retorno:

`PaymentsAPI → PaymentProcessedEvent → CatalogAPI`

---

# Segurança

O microsserviço utiliza autenticação baseada em JWT.

Header necessário para endpoints protegidos:

`Authorization: Bearer <token>`

Dependendo da rota, pode haver uso de policies como `Admin`.

---

# Arquitetura Completa

Este serviço faz parte da arquitetura composta por:

- UsersAPI
- CatalogAPI
- PaymentsAPI
- NotificationsAPI
- Orchestration

Execução integrada:

`docker compose up`

---

# Observações

Este microsserviço possui papel central no fluxo de compra, mas **não processa o pagamento diretamente**.  
O processamento é assíncrono e desacoplado, o que atende ao objetivo da Fase 2 de demonstrar integração por eventos e arquitetura distribuída.
