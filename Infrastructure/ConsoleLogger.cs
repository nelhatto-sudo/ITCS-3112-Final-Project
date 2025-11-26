namespace ITCS_3112_Final_Project;

public sealed class ConsoleLogger : ILogger
{
    private static ConsoleLogger? _instance = null;
    private ConsoleLogger() {}

    public static ConsoleLogger GetInstance()
    {
        if (_instance == null) { _instance = new ConsoleLogger(); }
        return _instance;
    }

    public void Info(string message)
    {
        Console.WriteLine($"[INFO] {message}");
    }

    public void Warn(string message)
    {
        Console.WriteLine($"[WARN] {message}");
    }

    public void Error(string message)
    {
        Console.WriteLine($"[ERROR] {message}");
    }
}