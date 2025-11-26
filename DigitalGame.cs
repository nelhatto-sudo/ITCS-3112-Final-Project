namespace ITCS_3112_Final_Project;

public class DigitalGame : Game
{
    public GameStoreEnum GameStore { get; }

    internal DigitalGame(int gameId, int displayId, GameStoreEnum gameStore)
        : base(gameId, displayId)
    {
        GameStore = gameStore;
    }

    public override void GetStorageDetails()
    {
        Console.WriteLine($"[Digital] GameId={GameId}, Store={GameStore}");
    }
}