using UnityEngine;

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}

public static class GameLogger
{
    private static bool loggingEnabled = true;
    private static LogLevel minimumLogLevel = LogLevel.Debug;
    
    public static void SetLoggingEnabled(bool enabled)
    {
        loggingEnabled = enabled;
    }
    
    public static void SetMinimumLogLevel(LogLevel level)
    {
        minimumLogLevel = level;
    }
    
    public static void Log(string message, LogLevel level = LogLevel.Info, string category = "Game")
    {
        if (!loggingEnabled || level < minimumLogLevel)
            return;
        
        string formattedMessage = $"[{category}] {message}";
        
        switch (level)
        {
            case LogLevel.Debug:
            case LogLevel.Info:
                Debug.Log(formattedMessage);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(formattedMessage);
                break;
            case LogLevel.Error:
                Debug.LogError(formattedMessage);
                break;
        }
    }
    
    // Convenience methods
    public static void LogNetwork(string message) => Log(message, LogLevel.Info, "Network");
    public static void LogPhysics(string message) => Log(message, LogLevel.Info, "Physics");
    public static void LogUI(string message) => Log(message, LogLevel.Info, "UI");
    public static void LogError(string message, string category = "Game") => Log(message, LogLevel.Error, category);
}
