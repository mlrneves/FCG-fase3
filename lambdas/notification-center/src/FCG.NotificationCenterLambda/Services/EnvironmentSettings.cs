namespace FCG.NotificationCenterLambda.Services;

public sealed class EnvironmentSettings
{
    public string EnvironmentName { get; init; } = "dev";
    public string NotificationMode { get; init; } = "log";
    public string SenderName { get; init; } = "FCG Games";
    public string SenderEmail { get; init; } = "noreply@fcg.local";

    public static EnvironmentSettings Load()
    {
        return new EnvironmentSettings
        {
            EnvironmentName = Environment.GetEnvironmentVariable("APP_ENVIRONMENT") ?? "dev",
            NotificationMode = Environment.GetEnvironmentVariable("NOTIFICATION_MODE") ?? "log",
            SenderName = Environment.GetEnvironmentVariable("NOTIFICATION_SENDER_NAME") ?? "FCG Games",
            SenderEmail = Environment.GetEnvironmentVariable("NOTIFICATION_SENDER_EMAIL") ?? "noreply@fcg.local"
        };
    }
}
