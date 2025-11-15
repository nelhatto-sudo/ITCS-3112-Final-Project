namespace ITCS_3112_Final_Project;

public class GameDisplay
{
    public int DisplayId { get; }
    public string Title { get; }
    public GenreEnum Genre { get; }
    public double Rating { get; }

    public GameDisplay(int displayId, string title, GenreEnum genre, double rating)
    {
        DisplayId = displayId;
        Title = title;
        Genre = genre;
        Rating = rating;
    }

    /*public override string ToString()
    {
        return $"{Title} ({Genre}, rating {Rating})";
    }*/
}