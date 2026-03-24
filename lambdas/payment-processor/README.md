# FCG Phase 3 - AWS Payment Processor Lambda (.NET 8)

Projeto pronto para a Fase 3 da pós, focado no requisito **serverless para processamento assíncrono de pagamentos**.

## O que este projeto faz

- Consome mensagens de uma fila **AWS SQS**
- Desserializa o evento `PurchaseCreated`
- Chama o `PaymentsAPI` interno para processar o pagamento
- Chama o `CatalogAPI` interno para atualizar o status da compra
- Propaga `x-internal-api-key` e `x-correlation-id`
- Retorna falhas parciais em lote para retry automático da SQS

## Fluxo esperado

1. `CatalogAPI` cria a compra com status `Pending`
2. `CatalogAPI` publica `PurchaseCreatedEvent` na SQS
3. Esta Lambda é acionada pela SQS
4. A Lambda chama:
   - `POST {PaymentsApiBaseUrl}/api/payments/internal/process`
   - `POST {CatalogApiBaseUrl}/api/internal/purchases/payment-result`
5. O `CatalogAPI` atualiza a compra para `Approved` ou `Rejected`

## Estrutura

```text
src/Fcg.PaymentProcessorLambda
├── Function.cs
├── Models
│   ├── CatalogPaymentResultRequest.cs
│   ├── PaymentProcessRequest.cs
│   ├── PaymentProcessResponse.cs
│   └── PurchaseCreatedEvent.cs
├── Fcg.PaymentProcessorLambda.csproj
└── aws-lambda-tools-defaults.json

template.yaml
.env.example
```

## Variáveis de ambiente

- `PAYMENTS_API_BASE_URL`
- `CATALOG_API_BASE_URL`
- `INTERNAL_API_KEY`
- `LOG_LEVEL` (opcional, default `Information`)
- `HTTP_TIMEOUT_SECONDS` (opcional, default `30`)

## Exemplo de mensagem esperada na SQS

```json
{
  "eventId": "cba85fef-bbe7-4b13-a06b-86b29d8e7b6d",
  "eventType": "PurchaseCreated",
  "occurredAt": "2026-03-23T19:00:00Z",
  "source": "catalog-api",
  "correlationId": "f7d8cbb9-2a5a-4477-a31f-4d8f2d1f5f2c",
  "purchaseId": 10,
  "userId": 3,
  "gameId": 7,
  "price": 199.90
}
```

## Deploy

### Opção 1: AWS SAM

```bash
sam build
sam deploy --guided
```

O `template.yaml` já cria:

- fila SQS `PurchaseCreatedQueue`
- Lambda `PaymentProcessorFunction`
- Event Source Mapping da fila para a Lambda

### Opção 2: Amazon Lambda Tools

Você também pode publicar só a função e depois conectar manualmente à SQS.

```bash
dotnet tool install -g Amazon.Lambda.Tools
cd src/Fcg.PaymentProcessorLambda
dotnet lambda deploy-function FcgPaymentProcessorLambda
```

## Ajustes necessários no seu ambiente AWS

Antes do deploy, configure no stack/console:

- URL/base interna do `PaymentsAPI`
- URL/base interna do `CatalogAPI`
- chave `x-internal-api-key`

## Observação importante

Este projeto foi montado para encaixar **no código que você já tem hoje**:

### PaymentsAPI
Endpoint esperado:
`POST /api/payments/internal/process`

Body esperado:
```json
{
  "purchaseId": 10,
  "userId": 3,
  "gameId": 7,
  "amount": 199.90,
  "correlationId": "..."
}
```

### CatalogAPI
Endpoint esperado:
`POST /api/internal/purchases/payment-result`

Body esperado:
```json
{
  "purchaseId": 10,
  "status": "Approved",
  "processedAt": "2026-03-23T19:00:00Z",
  "correlationId": "..."
}
```

## Próximo passo recomendado

Depois desta Lambda, o próximo passo natural é criar também uma **Notification Lambda** consumindo:

- `UserRegistered`
- `PaymentProcessed` ou resultado de compra aprovada

