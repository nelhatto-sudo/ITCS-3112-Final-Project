namespace ITCS_3112_Final_Project;

public abstract class Game
{
    public int GameId { get; }      // auto-incremented
    public int DisplayId { get; }

    protected Game(int gameId, int displayId)
    {
        GameId = gameId;
        DisplayId = displayId;
    }

    public GameDisplay? GetDisplay(ICatalog catalog) => catalog.GetGameDisplay(DisplayId);
    public string? GetTitle(ICatalog catalog)        => GetDisplay(catalog)?.Title;
    public GenreEnum? GetGenre(ICatalog catalog)     => GetDisplay(catalog)?.Genre;
    public double? GetRating(ICatalog catalog)       => GetDisplay(catalog)?.Rating;

    public abstract void GetStorageDetails();
}