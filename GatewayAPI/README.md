# GatewayAPI com YARP

Este pacote cria um gateway reverso em ASP.NET Core usando YARP para ficar na frente dos seus microsserviços.

## Rotas
- `/users/*` -> UsersAPI
- `/games/*` -> Catalog/Games API
- `/payments/*` -> PaymentsAPI

## O que ele já faz
- remove o prefixo da rota antes de encaminhar para o backend
- expõe Swagger do próprio gateway
- inclui endpoint `/health`
- inclui CORS liberado para facilitar seus testes
- pronto para Docker

## Como funciona
Exemplo:

Entrada no gateway:
`/users/api/Auth/login`

Saída para o backend:
`http://users-api:8080/api/Auth/login`

O mesmo vale para `/games/*` e `/payments/*`.

## Como usar localmente
Na pasta `GatewayAPI`:

```bash
dotnet restore
dotnet run
```

Depois acesse:
- `http://localhost:5090/swagger`
- `http://localhost:5090/health`

## Como usar com Docker
Monte o gateway na mesma rede Docker dos microsserviços e use estes nomes de host:
- `users-api`
- `catalog-api`
- `payments-api`

O appsettings já está preparado para isso.

## Exemplo de docker-compose
Veja o arquivo `docker-compose.gateway-example.yml`.

## Rotas finais esperadas
- `http://SEU-IP:5000/users/swagger/index.html`
- `http://SEU-IP:5000/users/api/Auth/login`
- `http://SEU-IP:5000/games/swagger/index.html`
- `http://SEU-IP:5000/payments/swagger/index.html`

## Observação
Se os nomes dos seus containers forem diferentes, ajuste os endereços em `appsettings.json`.