using ITCS_3112_Final_Project;

namespace ITCS_3112_Final_Project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = ConsoleLogger.GetInstance(); 
            
            // interfaces 
            ICatalog catalog = new Catalog();
            IGameRepository gameRepository = new InMemoryGameRepository();
            IUserRepository userRepository = new InMemoryUserRepository();

            // Service layer 
            GameLibraryService gameLibraryService = new GameLibraryService(
                catalog,
                gameRepository,
                userRepository,
                logger);

            // UI layer (console menu)
            IMenuActions menu = new MenuActions(gameLibraryService, logger);
            
            menu.Run();
        }
    }
}