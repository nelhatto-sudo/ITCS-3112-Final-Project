namespace ITCS_3112_Final_Project;

public class GameFactory
{   
    private static int _nextGameId = 1;

    public static PhysicalGame CreatePhysicalGame(int displayId, string location)
    {
        var id = _nextGameId++;
        return new PhysicalGame(id, displayId, location);
    }

    public static DigitalGame CreateDigitalGame(int displayId, GameStoreEnum gameStore)
    {
        var id = _nextGameId++;
        return new DigitalGame(id, displayId, gameStore);
    }
}