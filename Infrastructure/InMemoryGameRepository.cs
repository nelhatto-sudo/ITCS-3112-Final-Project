namespace ITCS_3112_Final_Project;

public class InMemoryGameRepository : IGameRepository
{
    private readonly Dictionary<int, List<GameDetails>> _libraries = new();

    public IReadOnlyList<GameDetails> GetUserLibrary(int userId)
    {
        if (!_libraries.TryGetValue(userId, out var list))
            return Array.Empty<GameDetails>();

        return list.AsReadOnly();
    }

    public void AddGame(int userId, Game game, StatusEnum status)
    {
        if (!_libraries.TryGetValue(userId, out var list))
        {
            list = new List<GameDetails>();
            _libraries[userId] = list;
        }

        list.Add(new GameDetails(game, status));
    }

    public bool RemoveGame(int userId, int gameId)
    {
        if (!_libraries.TryGetValue(userId, out var list))
            return false;

        var index = list.FindIndex(gd => gd.Game.GameId == gameId);
        if (index < 0) return false;

        list.RemoveAt(index);
        return true;
    }

    public bool UpdateStatus(int userId, int gameId, StatusEnum newStatus)
    {
        if (!_libraries.TryGetValue(userId, out var list))
            return false;

        var index = list.FindIndex(gd => gd.Game.GameId == gameId);
        if (index < 0) return false;

        var old = list[index];
        list[index] = old with { Status = newStatus };
        return true;
    }
}