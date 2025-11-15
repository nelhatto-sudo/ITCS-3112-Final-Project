namespace ITCS_3112_Final_Project;

public class Catalog : ICatalog
{
    private readonly Dictionary<int, GameDisplay> _allGames =
        new Dictionary<int, GameDisplay>();

    private int _nextDisplayId = 1;

    public GameDisplay CreateGameDisplay(string title, GenreEnum genre, double rating)
    {
        var displayId = _nextDisplayId++;
        var display = new GameDisplay(displayId, title, genre, rating);
        _allGames[displayId] = display;
        return display;
    }

    public GameDisplay? GetGameDisplay(int displayId)
    {
        return _allGames.TryGetValue(displayId, out var d) ? d : null;
    }
}