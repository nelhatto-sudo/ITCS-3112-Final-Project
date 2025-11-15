namespace ITCS_3112_Final_Project;

public class DigitalGame : Game
{
    public string GameStore { get; }

    internal DigitalGame(int gameId, int displayId, string gameStore)
        : base(gameId, displayId)
    {
        GameStore = gameStore;
    }

    public override void GetStorageDetails()
    {
        Console.WriteLine($"[Digital] GameId={GameId}, Store={GameStore}");
    }
}