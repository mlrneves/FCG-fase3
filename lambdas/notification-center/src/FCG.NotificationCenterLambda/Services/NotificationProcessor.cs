using FCG.NotificationCenterLambda.Models;

namespace FCG.NotificationCenterLambda.Services;

public sealed class NotificationProcessor(EnvironmentSettings settings, NotificationSink sink)
{
    public async Task ProcessAsync(NotificationEnvelope envelope, CancellationToken cancellationToken)
    {
        var (subject, body) = BuildMessage(envelope);
        await sink.SendAsync(envelope, subject, body, cancellationToken);
    }

    private static (string Subject, string Body) BuildMessage(NotificationEnvelope envelope)
    {
        return envelope.EventType switch
        {
            "UserRegistered" => (
                "Bem-vindo à FCG Games",
                $"Olá! Seu cadastro foi concluído com sucesso. UserId: {envelope.UserId}."
            ),
            "PaymentProcessed" => (
                $"Pagamento {NormalizeStatus(envelope.Status)}",
                $"Sua compra {envelope.PurchaseId} do jogo {envelope.GameId} teve o pagamento {NormalizeStatus(envelope.Status)}. Valor: {envelope.Amount?.ToString("0.00") ?? "0.00"}."
            ),
            _ => (
                $"Evento {envelope.EventType}",
                "Um novo evento foi recebido e registrado pelo centro de notificações."
            )
        };
    }

    private static string NormalizeStatus(string? status)
        => string.IsNullOrWhiteSpace(status) ? "processado" : status!.Trim().ToLowerInvariant() switch
        {
            "approved" => "aprovado",
            "rejected" => "rejeitado",
            _ => status
        };
}
