namespace ITCS_3112_Final_Project;

public class BaseStrategy : IRecommendationStrategy
{
    private readonly ICatalog _catalog;
    private readonly IGameRepository _gameRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public BaseStrategy(
        ICatalog catalog,
        IGameRepository gameRepository,
        IUserRepository userRepository,
        ILogger logger)
    {
        _catalog = catalog;
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public List<GameDisplay> RecommendGames(int userId)
    {
        // Placeholder for now – will implement later.
        // You could, for example, return some “top rated” games
        // or a genre-based recommendation here.
        return new List<GameDisplay>();
    }
}