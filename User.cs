namespace ITCS_3112_Final_Project;

public class User
{
    public int UserId { get; }             
    public string UserName { get; set; }

    // Key: displayId (from GameDisplay)
    // Value: rating (int 1–5, or whatever scale you pick)
    private readonly Dictionary<int, int> _ratings = new();

    public User(int userId, string userName)
    {
        UserId = userId;
        UserName = userName;
    }
    
    public void RateGame(int displayId, int rating)
    {
        // rating = Math.Clamp(rating, 1, 5);
        _ratings[displayId] = rating;
    }
    
    public bool TryGetRating(int displayId, out int rating)
    {
        return _ratings.TryGetValue(displayId, out rating);
    }
}