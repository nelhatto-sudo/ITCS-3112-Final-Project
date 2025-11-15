using System.Security.AccessControl;

namespace ITCS_3112_Final_Project;

public class PhysicalGame : Game
{
    public string Location { get; private set; }

    internal PhysicalGame(int gameId, int displayId, string location)
        : base(gameId, displayId)
    {
        Location = location;
    }

    public void ChangeLocation(string newLocation) => Location = newLocation;

    public override void GetStorageDetails()
    {
        Console.WriteLine($"[Physical] GameId={GameId}, Location={Location}");
    }
}