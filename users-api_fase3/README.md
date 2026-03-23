
# UsersAPI

## Visão Geral

O **UsersAPI** é o microsserviço responsável pelo **gerenciamento de usuários e autenticação do sistema FCG**.

Ele centraliza todas as responsabilidades relacionadas a:

- cadastro de usuários
- autenticação
- autorização
- emissão de tokens JWT
- publicação de eventos relacionados a usuários

Este microsserviço faz parte da arquitetura baseada em **microserviços e comunicação assíncrona com eventos**, utilizando **RabbitMQ** e **MassTransit**.

---

# Responsabilidade do Microsserviço

O **UsersAPI** possui as seguintes responsabilidades:

- gerenciar o cadastro de usuários
- autenticar usuários
- gerar tokens JWT
- aplicar regras de autorização
- publicar eventos quando usuários são criados
- fornecer endpoints seguros para gestão de usuários

Este serviço **não gerencia jogos, compras ou pagamentos**, pois essas responsabilidades pertencem a outros microsserviços da arquitetura.

---

# Evento Publicado

## UserCreatedEvent

Publicado quando um novo usuário é criado.

Objetivo:

- permitir que outros microsserviços reajam à criação de um usuário

Exemplos de consumidores:

- NotificationsAPI → registrar notificação de novo usuário
- outros serviços futuros que precisem reagir à criação de usuário

Este microsserviço **não consome eventos**, atuando apenas como **publisher** no barramento de mensagens.

---

# Arquitetura

O projeto segue uma arquitetura em camadas.

## Core

Contém as regras de negócio e contratos da aplicação.

Responsabilidades:

- entidades do domínio
- interfaces de repositório
- interfaces de serviços
- eventos de domínio
- enums e contratos de negócio

Objetivo:

- manter a lógica de negócio isolada de detalhes de infraestrutura

---

## Infrastructure

Contém implementações técnicas necessárias para persistência e integração.

Responsabilidades:

- implementação dos repositórios
- configuração do Entity Framework
- acesso ao banco de dados
- implementação de serviços técnicos

Objetivo:

- isolar detalhes de banco e infraestrutura do domínio da aplicação

---

## UsersAPI (Web API)

Camada responsável pela exposição da API HTTP.

Responsabilidades:

- controllers
- autenticação JWT
- configuração do MassTransit
- configuração de dependências
- middleware
- configuração do Swagger

Objetivo:

- expor endpoints REST para consumo externo

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

FCGUsers

As migrations são aplicadas automaticamente na inicialização da aplicação através de:

Database.Migrate()

---

# Usuário Administrador Inicial

Na inicialização da aplicação é criado automaticamente um **usuário administrador padrão**, caso ele ainda não exista.

Configuração:

AdminUser
- Name
- Email
- Password

---

# Variáveis de Ambiente

## Banco

ConnectionStrings__ConnectionString

## RabbitMQ

RabbitMq__Host  
RabbitMq__Username  
RabbitMq__Password  

## JWT

Jwt__Issuer  
Jwt__Key  

## Admin inicial

AdminUser__Name  
AdminUser__Email  
AdminUser__Password  

---

# Execução Local

dotnet restore  
dotnet build  
dotnet run  

Swagger:

http://localhost:{porta}/swagger

---

# Docker

Build da imagem:

docker build -t fcg-users-api .

Executar container:

docker run -p 5001:8080 fcg-users-api

---

# Kubernetes

Manifestos disponíveis na pasta:

/k8s

Arquivos:

- users-api-configmap.yaml
- users-api-secret.yaml
- users-api-deployment.yaml
- users-api-service.yaml

---

# Comunicação Assíncrona

Fluxo de eventos:

UsersAPI → UserCreatedEvent → NotificationsAPI

---

# Segurança

Autenticação baseada em JWT.

Header necessário:

Authorization: Bearer <token>

Alguns endpoints exigem a policy:

Admin

---

# Arquitetura Completa

Este serviço faz parte da arquitetura:

- UsersAPI
- CatalogAPI
- PaymentsAPI
- NotificationsAPI
- Orchestration

Execução integrada:

docker compose up
