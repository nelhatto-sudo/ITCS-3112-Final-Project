namespace ITCS_3112_Final_Project;

public class PopularityRecommendationStrategy : IRecommendationStrategy
{
    private readonly ICatalog _catalog;
    private readonly IGameRepository _gameRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public PopularityRecommendationStrategy(ICatalog catalog ,IGameRepository gameRepository, IUserRepository userRepository, ILogger logger)
    {
        _catalog = catalog;
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public List<GameDisplay> RecommendGames(int userId)
    {
        const int maxResults = 5;

        // 1. Build a set of display IDs the current user already owns
        var userLibrary = _gameRepository.GetUserLibrary(userId);
        var ownedDisplayIds = new HashSet<int>(
            userLibrary.Select(gd => gd.Game.DisplayId)
        );

        // 2. Take all catalog games, exclude owned games and those with no rating,
        //    then sort by the existing average rating on GameDisplay.
        var recommendations = _catalog
            .GetAll()
            .Where(d => !ownedDisplayIds.Contains(d.DisplayId))
            .Where(d => d.Rating > 0)                 // skip unrated games
            .OrderByDescending(d => d.Rating)         // highest average rating first
            .ThenBy(d => d.Title)                     // stable tiebreaker
            .Take(maxResults)
            .ToList();

        return recommendations;
    }
}