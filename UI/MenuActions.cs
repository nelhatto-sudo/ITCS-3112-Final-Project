using System;
using System.Collections.Generic;
using ITCS_3112_Final_Project;

public class MenuActions : IMenuActions
{
    private readonly GameLibraryService _service;
    private readonly ILogger _logger;

    private User? _currentUser;   

    public MenuActions(GameLibraryService service, ILogger logger)
    {
        _service = service;
        _logger = logger;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            if (_currentUser == null)
                RunGuestMenu();
            else
                RunUserMenu();
        }
    }

    // ==================== GUEST MENU ====================

    private void RunGuestMenu()
    {
        Console.WriteLine("=== Game Library ===");
        Console.WriteLine("Current user: [none]");
        Console.WriteLine();
        Console.WriteLine("1. View game catalog");
        Console.WriteLine("2. Register new user");
        Console.WriteLine("3. Login");
        Console.WriteLine("4. View all users");
        Console.WriteLine("0. Exit");
        Console.Write("Choice: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ViewCatalog();
                break;
            case "2":
                RegisterUser();
                break;
            case "3":
                Login();
                break;
            case "4":
                ViewUsers();
                break;
            case "0":
                Environment.Exit(0);
                break;
            default:
                _logger.Warn("Unknown choice.");
                break;
        }

        Pause();
    }

    // ==================== LOGGED-IN MENU ====================

    private void RunUserMenu()
    {
        Console.WriteLine("=== Game Library ===");
        Console.WriteLine($"Current user: [{_currentUser!.UserId}] {_currentUser.UserName}");
        Console.WriteLine();
        Console.WriteLine("1. View my games");
        Console.WriteLine("2. Add game to my library");
        Console.WriteLine("3. Update game status");
        Console.WriteLine("4. Remove game from my library");
        Console.WriteLine("5. Rate a game in my library");
        Console.WriteLine("6. Recommendations");
        Console.WriteLine("7. View game catalog");
        Console.WriteLine("8. Logout");
        Console.WriteLine("0. Exit");
        Console.Write("Choice: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                ViewMyGames();
                break;
            case "2":
                AddGameToMyLibrary();
                break;
            case "3":
                UpdateGameStatus();
                break;
            case "4":
                RemoveGameFromMyLibrary();
                break;
            case "5":
                RateGameInMyLibrary();
                break;
            case "6":
                RunRecommendationsMenu();
                break;
            case "7":
                ViewCatalog();
                break;
            case "8":
                Logout();
                break;
            case "0":
                Environment.Exit(0);
                break;
            default:
                _logger.Warn("Unknown choice.");
                break;
        }

        Pause();
    }

    // ==================== SUB-MENUS ====================

    private void ViewMyGames()
    {
        if (_currentUser == null)
        {
            Console.WriteLine("Not logged in.");
            return;
        }

        Console.WriteLine("View which games?");
        Console.WriteLine("1. OwnedAvailable");
        Console.WriteLine("2. OwnedLoanedOut");
        Console.WriteLine("3. Borrowed");
        Console.WriteLine("4. All games");
        Console.Write("Choice: ");
        var choice = Console.ReadLine();

        StatusEnum? filter = choice switch
        {
            "1" => StatusEnum.OwnedAvailable,
            "2" => StatusEnum.OwnedLoanedOut,
            "3" => StatusEnum.Borrowed,
            _   => null
        };

        var games = _service.GetUserGamesView(_currentUser.UserId, filter);
        if (games.Count == 0)
        {
            Console.WriteLine("No games match this category.");
            return;
        }

        Console.WriteLine("Game Id | Name | Status | Genre | Rating");
        foreach (var g in games)
        {
            Console.WriteLine($"{g.GameId} |{g.Title} | {g.Status} | {g.Genre} | {g.Rating:F1}");
        }
    }

    private void RunRecommendationsMenu()
    {
        if (_currentUser == null)
        {
            Console.WriteLine("Not logged in.");
            return;
        }

        Console.WriteLine("=== Recommendations ===");
        Console.WriteLine("1. Base recommendation");
        Console.WriteLine("2. Dot product recommendation");
        Console.WriteLine("0. Back");
        Console.Write("Choice: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                BaseRecommendation();
                break;
            case "2":
                DotProductRecommendation();
                break;
            case "0":
                return;
            default:
                _logger.Warn("Unknown choice.");
                break;
        }
    }

    // ==================== GUEST ACTIONS ====================

    private void RegisterUser()
    {
        Console.Write("Enter user name: ");
        var name = Console.ReadLine() ?? string.Empty;
        var user = _service.RegisterUser(name);
        Console.WriteLine($"Created user [{user.UserId}] {user.UserName}");
    }

    private void ViewUsers()
    {
        var users = _service.GetAllUsers();
        if (users.Count == 0)
        {
            Console.WriteLine("No users.");
            return;
        }

        foreach (var u in users)
            Console.WriteLine($"{u.UserId}: {u.UserName}");
    }

    private void Login()
    {
        Console.Write("Enter user id to log in: ");
        if (!int.TryParse(Console.ReadLine(), out var id))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        var user = _service.GetUserById(id);
        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        _currentUser = user;
        Console.WriteLine($"Logged in as [{user.UserId}] {user.UserName}");
    }

    private void Logout()
    {
        if (_currentUser == null)
        {
            Console.WriteLine("No user is currently logged in.");
            return;
        }

        Console.WriteLine($"Logged out [{_currentUser.UserId}] {_currentUser.UserName}");
        _currentUser = null;
    }

    // ==================== CATALOG ACTIONS ====================

    private void ViewCatalog()
    {
        var games = _service.GetCatalogGames();
        if (games.Count == 0)
        {
            Console.WriteLine("Catalog is empty.");
            return;
        }

        Console.WriteLine("Name | Genre | Rating");
        foreach (var g in games)
            Console.WriteLine($"{g.DisplayId} | {g.Title} | {g.Genre} | {g.Rating:F1}");
    }

    // ==================== MY LIBRARY ACTIONS ====================

    private void AddGameToMyLibrary()
    {
        if (_currentUser == null)
        {
            Console.WriteLine("Not logged in.");
            return;
        }

        Console.Write("Enter catalog display id to add: ");
        if (!int.TryParse(Console.ReadLine(), out var displayId))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        Console.WriteLine("Initial status?");
        Console.WriteLine("1. OwnedAvailable");
        Console.WriteLine("2. OwnedLoanedOut");
        Console.WriteLine("3. Borrowed");
        Console.Write("Choice: ");
        var choice = Console.ReadLine();

        StatusEnum status = choice switch
        {
            "1" => StatusEnum.OwnedAvailable,
            "2" => StatusEnum.OwnedLoanedOut,
            "3" => StatusEnum.Borrowed,
            _   => StatusEnum.OwnedAvailable
        };
        
        Console.WriteLine("Game form");
        Console.WriteLine("1. Physical game");
        Console.WriteLine("2. Digital game");
        Console.Write("Choice: ");
        choice = Console.ReadLine();

        Game game;
        bool success = false; 
        if (choice == "1") {
            game = GameFactory.CreatePhysicalGame(displayId, "");
            success = _service.AddGameToUserLibrary(_currentUser.UserId, game, status);
        }
        else {
            Console.WriteLine("Choose game store");
            Console.WriteLine("1. Steam");
            Console.WriteLine("2. Xbox");
            Console.WriteLine("3. Playstation");
            Console.WriteLine("4. EpicGames");
            Console.WriteLine("5. Origin");
            Console.WriteLine("6. Other");
            Console.Write("Choice: ");
            choice = Console.ReadLine();
            
            GameStoreEnum store = choice switch
            {
                "1" => GameStoreEnum.Steam,
                "2" => GameStoreEnum.Xbox,
                "3" => GameStoreEnum.Playstation,
                "4" => GameStoreEnum.EpicGames,
                "5" => GameStoreEnum.Origin,
                _   => GameStoreEnum.Other
            };
            
            game = GameFactory.CreateDigitalGame(displayId, store);
            success = _service.AddGameToUserLibrary(_currentUser.UserId, game, status);
        }
        
        Console.WriteLine(success ? "Game added to your library." : "Failed to add game.");
    }

    private void UpdateGameStatus()
    {
        if (_currentUser == null)
        {
            Console.WriteLine("Not logged in.");
            return;
        }

        Console.Write("Enter game id in your library: ");
        if (!int.TryParse(Console.ReadLine(), out var gameId))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        Console.WriteLine("New status?");
        Console.WriteLine("1. OwnedAvailable");
        Console.WriteLine("2. OwnedLoanedOut");
        Console.WriteLine("3. Borrowed");
        Console.Write("Choice: ");
        var choice = Console.ReadLine();

        StatusEnum status = choice switch
        {
            "1" => StatusEnum.OwnedAvailable,
            "2" => StatusEnum.OwnedLoanedOut,
            "3" => StatusEnum.Borrowed,
            _   => StatusEnum.OwnedAvailable
        };

        var success = _service.UpdateGameStatus(_currentUser.UserId, gameId, status);
        Console.WriteLine(success
            ? "Status updated."
            : "Failed to update status.");
    }

    private void RemoveGameFromMyLibrary()
    {
        if (_currentUser == null)
        {
            Console.WriteLine("Not logged in.");
            return;
        }

        Console.Write("Enter game id in your library to remove: ");
        if (!int.TryParse(Console.ReadLine(), out var gameId))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        var success = _service.RemoveGameFromUserLibrary(_currentUser.UserId, gameId);
        Console.WriteLine(success
            ? "Game removed from your library."
            : "Failed to remove game.");
    }

    private void RateGameInMyLibrary()
    {
        if (_currentUser == null)
        {
            Console.WriteLine("Not logged in.");
            return;
        }

        Console.Write("Enter game id in your library to rate: ");
        if (!int.TryParse(Console.ReadLine(), out var gameId))
        {
            Console.WriteLine("Invalid id.");
            return;
        }

        Console.Write("Enter rating (1–5): ");
        if (!int.TryParse(Console.ReadLine(), out var rating))
        {
            Console.WriteLine("Invalid rating.");
            return;
        }

        var success = _service.RateGameInLibrary(_currentUser.UserId, gameId, rating);
        Console.WriteLine(success
            ? "Rating recorded."
            : "Failed to record rating.");
    }

    // ==================== RECOMMENDATION ACTIONS ====================

    private void BaseRecommendation()
    {
        if (_currentUser == null)
        {
            Console.WriteLine("Not logged in.");
            return;
        }

        var recs = _service.GetBaseRecommendations(_currentUser.UserId);
        if (recs.Count == 0)
        {
            Console.WriteLine("No base recommendations available.");
            return;
        }

        Console.WriteLine("Top Recommended Games You Don’t Own: ");
        foreach (var g in recs)
            Console.WriteLine($"{g.DisplayId} | {g.Title} | {g.Genre} | {g.Rating:F1}");
    }

    private void DotProductRecommendation()
    {
        if (_currentUser == null)
        {
            Console.WriteLine("Not logged in.");
            return;
        }

        var recs = _service.GetDotProductRecommendations(_currentUser.UserId);
        if (recs.Count == 0)
        {
            Console.WriteLine("No dot product recommendations available.");
            return;
        }

        Console.WriteLine("Top Recommended Games You Don’t Own: ");
        foreach (var g in recs)
            Console.WriteLine($"{g.DisplayId}| {g.Title} | {g.Genre} | {g.Rating:F1}");
    }

    // ==================== UTILITY ====================

    private void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}


