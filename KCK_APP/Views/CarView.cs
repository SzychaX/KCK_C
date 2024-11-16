using KCK_APP.Controllers;
using KCK_APP.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace KCK_APP.Views
{
    public class CarView
    {
        private readonly CarController _carController;

        public CarView(CarController carController)
        {
            _carController = carController;
        }

        public void DisplayMenu()
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Wybierz opcje")
                        .AddChoices("Dodaj samochod", "Pokaz katalog", "Wyszukiwanie", "Zakoncz"));

                switch (choice)
                {
                    case "Dodaj samochod":
                        AddCar();
                        break;
                    case "Pokaz katalog":
                        ShowCarCatalog();
                        break;
                    case "Wyszukiwanie":
                        SearchCarsMenuLive();
                        break;
                    case "Zakoncz":
                        Environment.Exit(0);
                        break;
                }
            }
        }

        public void AddCar()
        {
            var make = AnsiConsole.Ask<string>("Marka pojazdu: ");
            var model = AnsiConsole.Ask<string>("Model pojazdu: ");
            var year = AnsiConsole.Ask<int>("Rocznik samochodu: ");
            var engine = AnsiConsole.Ask<int>("Silnik samochodu: ");
            var horsepower = AnsiConsole.Ask<int>("Moc samochodu w KM: ");
            var mileage = AnsiConsole.Ask<int>("Przebieg samochodu: ");
            var body = AnsiConsole.Ask<string>("Rodzaj nadwozia: ");
            var color = AnsiConsole.Ask<string>("Kolor samochodu: ");
            var price = AnsiConsole.Ask<int>("Cena: ");

            var car = new Car()
            {
                Make = make,
                Model = model,
                Year = year,
                Engine = engine,
                HorsePower = horsepower,
                Mileage = mileage,
                Body = body,
                Color = color,
                Price = price
            };

            _carController.AddCar(car);
            AnsiConsole.MarkupLine("[green]Samochod zostal dodany do katalogu![/]");
        }

        public void ShowCarCatalog()
        {
            Console.Clear();
            List<Car> cars = _carController.GetAllCars();
            if (cars.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Brak samochodow w katalogu.[/]");
            }
            else
            {
                var table = CreateCarTable();

                foreach (var car in cars)
                {
                    table.AddRow(car.Id.ToString(), car.Make, car.Model, car.Year.ToString(), car.Engine.ToString(), car.HorsePower.ToString(), car.Mileage.ToString(), car.Body, car.Color, car.Price.ToString());
                }
                AnsiConsole.Render(table);
            }
        }
        
        public void SearchCars()
        {
            // Wpisz markę (można pozostawić puste)
            var searchMake = AnsiConsole.Prompt(new TextPrompt<string>("Wpisz markę [gray](pomiń, jeśli dowolna)[/]") { AllowEmpty = true });

            // Maksymalny przebieg (można pozostawić puste)
            decimal? maxMileage = null;
            var maxMileageInput = AnsiConsole.Prompt(new TextPrompt<string>("Maksymalny przebieg [gray](pomiń, jeśli dowolny)[/]: ").AllowEmpty());
            if (!string.IsNullOrEmpty(maxMileageInput))
            {
                maxMileage = decimal.Parse(maxMileageInput);
            }

            // Minimalna moc (można pozostawić puste)
            int? minHorsePower = null;
            var minHorsePowerInput = AnsiConsole.Prompt(new TextPrompt<string>("Minimalna moc w KM [gray](pomiń, jeśli dowolna)[/]: ").AllowEmpty());
            if (!string.IsNullOrEmpty(minHorsePowerInput))
            {
                minHorsePower = int.Parse(minHorsePowerInput);
            }

            // Przekazanie danych do kontrolera
            var results = _carController.SearchCars(searchMake, maxMileage, minHorsePower);
            if (results.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Nie znaleziono samochodów według podanych kryteriów.[/]");

            }
            else
            {
                var table = CreateCarTable();

                foreach (var car in results)
                {
                    table.AddRow(car.Id.ToString(), car.Make, car.Model, car.Year.ToString(), car.Engine.ToString(), car.HorsePower.ToString(), car.Mileage.ToString(), car.Body, car.Color, car.Price.ToString());
                }
                AnsiConsole.Render(table);
            }
        }

        public void SearchCarsMenuLive()
{
    string make = null;
    string body = null;
    string color = null;
    int? minYear = null;
    int? maxYear = null;
    decimal? minMileage = null;
    decimal? maxMileage = null;
    decimal? minPrice = null;
    decimal? maxPrice = null;

    // Pobierz unikalne wartości do menu
    var makes = _carController.GetUniqueMakes();
    var bodies = _carController.GetUniqueBodies();
    var colors = _carController.GetUniqueColors();

    // Funkcja aktualizująca tabelę wyników
    void UpdateTable()
    {
        var filteredCars = _carController.GetFilteredCars(make, body, minYear, maxYear, minMileage, maxMileage, minPrice, maxPrice, color);

        var table = CreateCarTable();

        foreach (var car in filteredCars)
        {
            table.AddRow(car.Id.ToString(), car.Make, car.Model, car.Year.ToString(), car.Engine.ToString(),
                         car.Mileage.ToString(), car.HorsePower.ToString(), car.Body, car.Color, car.Price.ToString());
        }

        AnsiConsole.Clear(); // Czyść ekran konsoli
        AnsiConsole.MarkupLine("[green]Dynamiczna tabela wyników wyszukiwania:[/]");
        AnsiConsole.Render(table);
    }

    // Pierwsze wywołanie: Wyświetl wszystkie wyniki
    UpdateTable();

    // Pętla obsługi kryteriów filtrowania
    while (true)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Co chcesz zmienić?")
                .AddChoices("Marka", "Nadwozie", "Kolor", "Minimalny rocznik", "Maksymalny rocznik",
                            "Minimalny przebieg", "Maksymalny przebieg", 
                            "Minimalna cena", "Maksymalna cena", "Powrót"));

        switch (choice)
        {
            case "Marka":
                var safeMakes = makes.Select(m => Markup.Escape(m)).ToList();
                make = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Wybierz markę:")
                        .AddChoices(safeMakes)
                        .PageSize(10)
                );

                if (make == "Wyczyść filtr") make = null;
                break;

            case "Nadwozie":
                var safeBodies = bodies.Select(b => Markup.Escape(b)).ToList();
                body = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Wybierz nadwozie:")
                        .AddChoices(safeBodies)
                        .PageSize(10)
                );
                if (body == "[Wyczyść filtr]") body = null;
                break;

            case "Kolor":
                var safeColors = colors.Select(c => Markup.Escape(c)).ToList();
                color = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Wybierz kolor:")
                        .AddChoices(safeColors)
                        .PageSize(10)
                );
                if (color == "[Wyczyść filtr]") color = null;
                break;

            case "Minimalny rocznik":
                minYear = AnsiConsole.Ask<int?>("Podaj minimalny rocznik [gray](lub zostaw puste):[/]", null);
                break;

            case "Maksymalny rocznik":
                maxYear = AnsiConsole.Ask<int?>("Podaj maksymalny rocznik [gray](lub zostaw puste):[/]", null);
                break;

            case "Minimalny przebieg":
                minMileage = AnsiConsole.Ask<decimal?>("Podaj minimalny przebieg [gray](lub zostaw puste):[/]", null);
                break;

            case "Maksymalny przebieg":
                maxMileage = AnsiConsole.Ask<decimal?>("Podaj maksymalny przebieg [gray](lub zostaw puste):[/]", null);
                break;

            case "Minimalna cena":
                minPrice = AnsiConsole.Ask<decimal?>("Podaj minimalną cenę [gray](lub zostaw puste):[/]", null);
                break;

            case "Maksymalna cena":
                maxPrice = AnsiConsole.Ask<decimal?>("Podaj maksymalną cenę [gray](lub zostaw puste):[/]", null);
                break;

            case "Powrót":
                Console.Clear();
                return;
        }

        // Aktualizacja tabeli po każdej zmianie kryterium
        UpdateTable();
    }
}






        public Table CreateCarTable()
        {
            var carTable = new Table();
            carTable.AddColumn("Id");
            carTable.AddColumn("Marka");
            carTable.AddColumn("Model");
            carTable.AddColumn("Rocznik");
            carTable.AddColumn("Silnik");
            carTable.AddColumn("Moc");
            carTable.AddColumn("Przebieg");
            carTable.AddColumn("Nadwozie");
            carTable.AddColumn("Kolor");
            carTable.AddColumn("Cena");
            return carTable;
        }
        
    }
} 