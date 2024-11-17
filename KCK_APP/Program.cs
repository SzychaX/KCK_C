using KCK_APP.Controllers;
using KCK_APP.Services;
using KCK_APP.Views;
using Spectre.Console;
namespace KCK_APP
{
    class Program
    {
        static void Main(string[] args)
        {

            var dockerService = new DockerService();
            var databaseService = new DatabaseService();
            var carController = new CarController(databaseService);
            var carView = new CarView(carController);

            // Uruchomienie kontenera Docker
            dockerService.StartDockerContainerAsync().Wait();

            // Utworzenie tabeli w bazie danych
            databaseService.CreateTable();

            // Wyświetlenie menu
            carView.DisplayMenu();
        }
    }
}

