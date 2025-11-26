using System;
using System.Collections.Generic;
using System.Linq;

namespace ITCS_3112_Final_Project
{
    public record UserGameView(
        int GameId,
        int DisplayId,
        string Title,
        StatusEnum Status,
        GenreEnum Genre,
        double Rating);
    
    public class GameLibraryService
    {
        private readonly ICatalog _catalog;
        private readonly IGameRepository _gameRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public GameLibraryService(
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
        

        // ==================== USERS ====================
        public User RegisterUser(string userName)
        {
            var user = _userRepository.CreateUser(userName);
            _logger.Info($"Created user {user.UserId}: {user.UserName}");
            return user;
        }

        public IReadOnlyList<User> GetAllUsers() 
            => _userRepository.GetAll();

        public User? GetUserById(int userId) 
            => _userRepository.GetById(userId);
        

        // ==================== CATALOG (READ-ONLY) ====================
        public IReadOnlyList<GameDisplay> GetCatalogGames()
        {
            return _catalog.GetAll();
        }

        public GameDisplay? GetGameDisplay(int displayId) 
            => _catalog.GetGameDisplay(displayId);
        

        // ==================== USER LIBRARIES ====================
        public bool AddGameToUserLibrary(int userId, Game game, StatusEnum status)
        {
            _gameRepository.AddGame(userId, game, status);
            _logger.Info($"Added game (DisplayId={game.DisplayId}) to user {userId} with status {status}.");
            return true;
        }
        
        public bool UpdateGameStatus(int userId, int gameId, StatusEnum newStatus)
        {
            var success = _gameRepository.UpdateStatus(userId, gameId, newStatus);
            if (!success)
            {
                _logger.Warn($"Failed to update status. UserId={userId}, GameId={gameId}");
            }
            return success;
        }

        public bool RemoveGameFromUserLibrary(int userId, int gameId)
        {
            var success = _gameRepository.RemoveGame(userId, gameId);
            if (!success)
            {
                _logger.Warn($"Failed to remove game. UserId={userId}, GameId={gameId}");
            }
            return success;
        }
        
        public IReadOnlyList<UserGameView> GetUserGamesView(int userId, StatusEnum? statusFilter)
        {
            var library = _gameRepository.GetUserLibrary(userId);
            if (statusFilter is not null)
            {
                library = library
                    .Where(gd => gd.Status == statusFilter.Value)
                    .ToList();
            }

            if (library.Count == 0)
                return Array.Empty<UserGameView>();

            var catalogGames = _catalog.GetAll();
            var byDisplayId = catalogGames.ToDictionary(g => g.DisplayId);

            var result = new List<UserGameView>();

            foreach (var gd in library)
            {
                var displayId = gd.Game.DisplayId;
                if (!byDisplayId.TryGetValue(displayId, out var cd))
                    continue;

                var view = new UserGameView(
                    GameId: gd.Game.GameId,
                    DisplayId: cd.DisplayId,
                    Title: cd.Title,
                    Status: gd.Status,
                    Genre: cd.Genre,
                    Rating: cd.Rating // already the global average you maintain
                );

                result.Add(view);
            }

            return result;
        }

        // ==================== RATINGS ====================
        public bool RateGameInLibrary(int userId, int gameId, int rating)
        {
            if (rating < 1 || rating > 10)
            {
                _logger.Warn("Rating must be between 1 and 10.");
                return false;
            }

            var user = _userRepository.GetById(userId);
            if (user == null)
            {
                _logger.Warn($"User {userId} not found when rating a game.");
                return false;
            }

            var library = _gameRepository.GetUserLibrary(userId);
            var entry = library.FirstOrDefault(gd => gd.Game.GameId == gameId);
            if (entry.Equals(default(GameDetails)))
            {
                _logger.Warn($"Game {gameId} not found in user {userId}'s library.");
                return false;
            }

            var displayId = entry.Game.DisplayId;

            var display = _catalog.GetGameDisplay(displayId);
            if (display == null)
            {
                _logger.Warn($"DisplayId {displayId} not found in catalog when rating.");
                return false;
            }

            display.AddOrUpdateUserRating(userId, rating);
            user.RateGame(displayId, rating);

            _logger.Info($"User {userId} rated DisplayId {displayId} as {rating}.");
            return true;
        }

        // ==================== RECOMMENDATIONS ====================

        public List<GameDisplay> GetBaseRecommendations(int userId)
        {
            IRecommendationStrategy strategy =
                new BaseStrategy(_catalog, _gameRepository, _userRepository, _logger);

            return strategy.RecommendGames(userId);
        }

        public List<GameDisplay> GetDotProductRecommendations(int userId)
        {
            IRecommendationStrategy strategy =
                new DotProductStrategy(_catalog, _gameRepository, _userRepository, _logger);

            return strategy.RecommendGames(userId);
        }
    }
}