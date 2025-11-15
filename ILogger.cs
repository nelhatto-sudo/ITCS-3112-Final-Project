namespace ITCS_3112_Final_Project;

public interface ILogger
{
    void Info(string message);
    void Warn(string message);
    void Error(string message);
}