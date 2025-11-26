using System;
using System.Collections.Generic;
using System.IO;

namespace ITCS_3112_Final_Project;

public class TextFileDataSeeder : IDataSeeder
{
    private readonly string _gamesFilePath;
    private readonly string _usersFilePath;
    private readonly ILogger _logger;

    public TextFileDataSeeder(string gamesFilePath, string usersFilePath, ILogger logger)
    {
        _gamesFilePath = gamesFilePath;
        _usersFilePath = usersFilePath;
        _logger = logger;
    }

    public void Seed(ICatalog catalog, IUserRepository userRepository, IGameRepository gameRepository)
    {
        SeedCatalog(catalog);
        SeedUsersAndLibraries(catalog, userRepository, gameRepository);
    }

    // ==================== CATALOG ====================

    private void SeedCatalog(ICatalog catalog)
    {
        if (!File.Exists(_gamesFilePath))
        {
            _logger.Warn($"games file not found: {_gamesFilePath}");
            return;
        }

        foreach (var rawLine in File.ReadLines(_gamesFilePath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            // Expected: Title | Genre
            var parts = line.Split('|');
            if (parts.Length < 2)
            {
                _logger.Warn($"Invalid games line (expected 'Title | Genre'): {line}");
                continue;
            }

            var title = parts[0].Trim();
            var genreText = parts[1].Trim();

            if (!Enum.TryParse<GenreEnum>(genreText, ignoreCase: true, out var genre))
            {
                _logger.Warn($"Unknown genre '{genreText}' for game '{title}'. Skipping.");
                continue;
            }

            // Initial rating 0.0; will be updated by user ratings
            catalog.CreateGameDisplay(title, genre, rating: 0.0);
        }

        _logger.Info("Catalog seeded from games file.");
    }

    // ==================== USERS + LIBRARIES ====================

    private void SeedUsersAndLibraries(
        ICatalog catalog,
        IUserRepository userRepository,
        IGameRepository gameRepository)
    {
        if (!File.Exists(_usersFilePath))
        {
            _logger.Warn($"users file not found: {_usersFilePath}");
            return;
        }

        // Map username -> User object
        var usersByName = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);

        foreach (var rawLine in File.ReadLines(_usersFilePath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split('|');
            if (parts.Length < 2)
            {
                _logger.Warn($"Invalid users line: {line}");
                continue;
            }

            var recordType = parts[0].Trim();
            if (recordType.Equals("USER", StringComparison.OrdinalIgnoreCase))
            {
                // USER | UserName
                var userName = parts[1].Trim();
                if (usersByName.ContainsKey(userName))
                    continue;

                var user = userRepository.CreateUser(userName);
                usersByName[userName] = user;
            }
            else if (recordType.Equals("LIBRARY", StringComparison.OrdinalIgnoreCase))
            {
                // LIBRARY | UserName | DisplayId | GameType | StoreOrLocation | Status | Rating
                if (parts.Length < 7)
                {
                    _logger.Warn($"Invalid LIBRARY line (expected 7 columns): {line}");
                    continue;
                }

                var userName = parts[1].Trim();
                if (!usersByName.TryGetValue(userName, out var user))
                {
                    _logger.Warn($"LIBRARY line references unknown user '{userName}': {line}");
                    continue;
                }

                if (!int.TryParse(parts[2].Trim(), out var displayId))
                {
                    _logger.Warn($"Invalid DisplayId in line: {line}");
                    continue;
                }

                var gameTypeText = parts[3].Trim();
                var storeOrLocation = parts[4].Trim();
                var statusText = parts[5].Trim();
                var ratingText = parts[6].Trim();

                if (!Enum.TryParse<StatusEnum>(statusText, ignoreCase: true, out var status))
                {
                    _logger.Warn($"Invalid Status '{statusText}' in line: {line}");
                    continue;
                }

                if (!int.TryParse(ratingText, out var rating))
                {
                    _logger.Warn($"Invalid rating '{ratingText}' in line: {line}");
                    continue;
                }

                // Get GameDisplay from catalog
                var display = catalog.GetGameDisplay(displayId);
                if (display == null)
                {
                    _logger.Warn($"No GameDisplay with DisplayId={displayId} for line: {line}");
                    continue;
                }

                // Create Game instance (digital or physical)
                Game game;
                if (gameTypeText.Equals("D", StringComparison.OrdinalIgnoreCase))
                {
                    if (!Enum.TryParse<GameStoreEnum>(storeOrLocation, ignoreCase: true, out var store))
                    {
                        store = GameStoreEnum.Other;
                    }

                    game = GameFactory.CreateDigitalGame(displayId, store);
                }
                else if (gameTypeText.Equals("P", StringComparison.OrdinalIgnoreCase))
                {
                    game = GameFactory.CreatePhysicalGame(displayId, storeOrLocation);
                }
                else
                {
                    _logger.Warn($"Unknown GameType '{gameTypeText}' in line: {line}");
                    continue;
                }

                // Add to user's library
                gameRepository.AddGame(user.UserId, game, status);

                // Record rating:
                // 1) on the user side
                user.RateGame(displayId, rating);

                // 2) on the GameDisplay side (updates average)
                display.AddOrUpdateUserRating(user.UserId, rating);
            }
            else
            {
                _logger.Warn($"Unknown record type '{recordType}' in line: {line}");
            }
        }

        _logger.Info("Users and libraries seeded from users file.");
    }
}
