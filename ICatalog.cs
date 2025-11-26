namespace ITCS_3112_Final_Project;

public interface ICatalog
{
    GameDisplay? GetGameDisplay(int displayId);
    GameDisplay CreateGameDisplay(string title, GenreEnum genre, double rating);
    IReadOnlyList<GameDisplay> GetAll();
    
}