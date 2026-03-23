
# PaymentsAPI

## Visão Geral
O **PaymentsAPI** é o microsserviço responsável por **processar pagamentos de compras de jogos** dentro da arquitetura do sistema FCG.

Ele recebe eventos de compra publicados pelo `CatalogAPI`, simula o processamento do pagamento e publica o resultado para que outros microsserviços reajam ao resultado.

Arquitetura baseada em **eventos assíncronos usando RabbitMQ e MassTransit**.

---

# Responsabilidade do Microsserviço

O PaymentsAPI é responsável por:

- Consumir `OrderPlacedEvent`
- Processar o pagamento (simulado)
- Persistir o resultado no banco do serviço
- Publicar `PaymentProcessedEvent`
- Permitir consulta de pagamentos via API

Ele **não autentica usuários**, **não gerencia catálogo** e **não envia notificações**.

Essas responsabilidades pertencem respectivamente a:

- UsersAPI
- CatalogAPI
- NotificationsAPI

---

# Eventos

## Evento Consumido

### OrderPlacedEvent

Publicado pelo **CatalogAPI** quando um usuário tenta comprar um jogo.

Campos principais:

- PurchaseId
- UserId
- GameId
- Price

Objetivo:
iniciar o processamento do pagamento.

---

## Evento Publicado

### PaymentProcessedEvent

Publicado pelo **PaymentsAPI** após processar o pagamento.

Campos principais:

- PurchaseId
- UserId
- GameId
- Price
- Status (Approved / Rejected)
- ProcessedAt

Consumido por:

- CatalogAPI
- NotificationsAPI

---

# Arquitetura do Projeto

O projeto segue separação em camadas.

## Core

Contém:

- Entidades do domínio
- Interfaces de repositórios
- Interfaces de serviços
- Contratos de eventos

Objetivo:
manter a regra de negócio desacoplada de infraestrutura.

---

## Infrastructure

Contém:

- DbContext
- Configurações do EF Core
- Repositórios
- Consumers do MassTransit
- Implementação dos serviços

Objetivo:
isolar detalhes técnicos da aplicação.

---

## PaymentsAPI (Web)

Responsável por:

- Configuração do MassTransit
- Configuração de autenticação JWT
- Swagger
- Controllers
- Inicialização da aplicação

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

# Banco de Dados

Banco utilizado:

FCGPayments

Tabela principal:

Payments

Migrations são aplicadas automaticamente no startup usando:

Database.Migrate()

Isso permite execução direta com Docker e Kubernetes.

---

# Variáveis de Ambiente

## Banco

ConnectionStrings__ConnectionString

Exemplo:

Server=sqlserver;Database=FCGPayments;User Id=sa;Password=Your_password123;TrustServerCertificate=True

---

## RabbitMQ

RabbitMq__Host  
RabbitMq__Username  
RabbitMq__Password

---

## JWT

Jwt__Issuer  
Jwt__Key

O token é gerado pelo **UsersAPI**, mas validado aqui.

---

## Filas

Queues__OrderPlaced

Define o nome da fila que consome `OrderPlacedEvent`.

---

# Execução Local

```bash
dotnet restore
dotnet build
dotnet run
```

Swagger:

```
http://localhost:{porta}/swagger
```

---

# Docker

Build:

```
docker build -t fcg-payments-api .
```

Run:

```
docker run -p 5003:8080 fcg-payments-api
```

---

# Kubernetes

Os manifestos ficam na pasta:

```
/k8s
```

Arquivos:

- payments-api-configmap.yaml
- payments-api-secret.yaml
- payments-api-deployment.yaml
- payments-api-service.yaml

Eles garantem:

- gerenciamento de pods com Deployment
- configurações em ConfigMap
- segredos em Secret
- exposição do serviço via Service

---

# Fluxo de Compra

1. CatalogAPI publica `OrderPlacedEvent`
2. PaymentsAPI consome o evento
3. Pagamento é processado
4. PaymentsAPI publica `PaymentProcessedEvent`
5. CatalogAPI e NotificationsAPI reagem ao resultado

---

# Observações Importantes

- Contratos de eventos devem ser **idênticos entre serviços**
- Configurações sensíveis devem ficar em **Secrets**
- Configurações não sensíveis devem ficar em **ConfigMaps**
