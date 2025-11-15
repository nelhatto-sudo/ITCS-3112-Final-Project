namespace ITCS_3112_Final_Project;

public interface IGameRepository
{
    IReadOnlyList<GameDetails> GetUserLibrary(int userId);

    void AddGame(int userId, Game game, StatusEnum status);

    bool RemoveGame(int userId, int gameId);

    bool UpdateStatus(int userId, int gameId, StatusEnum newStatus);
}