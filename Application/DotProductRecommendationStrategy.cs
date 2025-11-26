using System;
using System.Collections.Generic;
using System.Linq;

namespace ITCS_3112_Final_Project;

public class DotProductRecommendationStrategy : IRecommendationStrategy
{
    private readonly ICatalog _catalog; 
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public DotProductRecommendationStrategy(ICatalog catalog, IGameRepository gameRepository, IUserRepository userRepository, ILogger logger)
    {
        _catalog = catalog;
        _userRepository = userRepository;
        _logger = logger;
    }

    public List<GameDisplay> RecommendGames(int userId)
    {
        var targetUser = _userRepository.GetById(userId);
        if (targetUser == null)
        {
            _logger.Warn($"User {userId} not found; cannot compute dot product recommendations.");
            return new List<GameDisplay>();
        }

        var targetRatings = targetUser.GetAllRatings();
        if (targetRatings.Count == 0)
        {
            _logger.Warn($"User {userId} has no ratings; dot product recommendation not available.");
            return new List<GameDisplay>();
        }

        var allUsers = _userRepository.GetAll();
        var otherUsers = allUsers.Where(u => u.UserId != userId).ToList();
        if (otherUsers.Count == 0)
        {
            _logger.Info("No other users in the system; cannot compute dot product recommendations.");
            return new List<GameDisplay>();
        }

        // 1. Find the user with the highest dot product against targetUser.
        User? bestMatchUser = null;
        double bestDot = double.MinValue;

        foreach (var other in otherUsers)
        {
            var otherRatings = other.GetAllRatings();
            if (otherRatings.Count == 0)
                continue;

            double dot = ComputeDotProduct(targetRatings, otherRatings);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestMatchUser = other;
            }
        }

        // If no positive or meaningful match found, bail out.
        if (bestMatchUser == null || bestDot <= 0)
        {
            _logger.Info($"No strong dot product match found for user {userId}.");
            return new List<GameDisplay>();
        }

        _logger.Info($"Your taste is similar to {bestMatchUser.UserName}.");

        // 2. Recommend games that the best-matching user rated, but target user has not.
        var targetRatedIds = new HashSet<int>(targetRatings.Keys);
        var bestRatings = bestMatchUser.GetAllRatings();
        

        var recommendations = new List<GameDisplay>();
        foreach (var kvp in bestRatings)
        {
            var displayId = kvp.Key;
            var rating = kvp.Value;

            if (rating < 3)
                continue;

            // Target user should not already have rated this
            if (targetRatedIds.Contains(displayId))
                continue;

            var gameDisplay = _catalog.GetGameDisplay(displayId);
            if (gameDisplay == null)
                continue;

            recommendations.Add(gameDisplay);
        }

        // Sort recommendations by how much the best-match user likes them (descending rating)
        recommendations = recommendations
            .OrderByDescending(gd => bestRatings.TryGetValue(gd.DisplayId, out var r) ? r : 0)
            .ToList();

        return recommendations;
    }

    // ---------- helpers ----------

    private static double ComputeDotProduct(
        IReadOnlyDictionary<int, int> a,
        IReadOnlyDictionary<int, int> b)
    {
        double sum = 0;

        // Iterate over the smaller dictionary for efficiency
        var (smaller, larger) = a.Count <= b.Count ? (a, b) : (b, a);

        foreach (var kvp in smaller)
        {
            var displayId = kvp.Key;
            var ratingA = kvp.Value;

            if (larger.TryGetValue(displayId, out var ratingB))
            {
                sum += ratingA * ratingB;
            }
        }

        return sum;
    }
}
