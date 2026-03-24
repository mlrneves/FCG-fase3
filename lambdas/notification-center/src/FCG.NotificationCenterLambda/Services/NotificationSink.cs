using FCG.NotificationCenterLambda.Models;

namespace FCG.NotificationCenterLambda.Services;

public sealed class NotificationSink(EnvironmentSettings settings)
{
    public Task SendAsync(NotificationEnvelope envelope, string subject, string body, CancellationToken cancellationToken)
    {
        Console.WriteLine($"""
        ================== NOTIFICATION CENTER ==================
        Environment: {settings.EnvironmentName}
        Mode: {settings.NotificationMode}
        From: {settings.SenderName} <{settings.SenderEmail}>
        To: {envelope.Email ?? "not-informed@fcg.local"}
        Subject: {subject}
        CorrelationId: {envelope.CorrelationId}
        EventType: {envelope.EventType}
        Body:
        {body}
        =========================================================
        """);

        return Task.CompletedTask;
    }
}
