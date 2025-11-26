namespace ITCS_3112_Final_Project;

public class GameDisplay
{
    public int DisplayId { get; }
    public string Title { get; }
    public GenreEnum Genre { get; }
    public double Rating { get; set; }  //Hold the Avg Game Rating
    
    private readonly Dictionary<int, int> _userRatings = new(); //Keeps track of all Users' Ratings for the Game
    
    public void AddOrUpdateUserRating(int userId, int rating) //Only update when rating changes
    {
        _userRatings[userId] = rating;
        Rating = _userRatings.Values.Average();
    }
    
    public GameDisplay(int displayId, string title, GenreEnum genre, double rating)
    {
        DisplayId = displayId;
        Title = title;
        Genre = genre;
        Rating = rating;
    }
    
}