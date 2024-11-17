using KCK_APP.Controllers;
using KCK_APP.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

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
                RenderTable(table);
            }
        }

        public void SearchCars()
        {
            // Wpisz markę (można pozostawić puste)
            var searchMake = AnsiConsole.Prompt(new TextPrompt<string>("Wpisz markę [gray](pomiń, jeśli dowolna)[/]") { AllowEmpty = true });

            // Maksymalny przebieg (można pozostawić puste)
            decimal? maxMileage = null;
            var maxMileageInput = AnsiConsole.Prompt(new TextPrompt<string>("Maksymalny przebieg [gray](pomiń, jeśli dowolna)[/]: ").AllowEmpty());
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
                RenderTable(table);
            }
        }

        public void SearchCarsMenuLive()
        {
            string make1 = null;
            string body1 = null;
            string color1 = null;
            int? minYear1 = null;
            int? maxYear1 = null;
            decimal? minMileage1 = null;
            decimal? maxMileage1 = null;
            decimal? minPrice1 = null;
            decimal? maxPrice1 = null;

            // Pobierz unikalne wartości do menu
            var makes = _carController.GetUniqueMakes();
            var bodies = _carController.GetUniqueBodies();
            var colors = _carController.GetUniqueColors();
            
            makes.Add("[Wyczyść filtr]");
            bodies.Add("[Wyczyść filtr]");
            colors.Add("[Wyczyść filtr]");

            // Funkcja aktualizująca tabelę wyników
            void UpdateTable()
            {
                var filteredCars = _carController.GetFilteredCars(make1, body1, minYear1, maxYear1, minMileage1, maxMileage1, minPrice1, maxPrice1, color1);
                
                Console.WriteLine($"Liczba znalezionych samochodów: {filteredCars.Count}");
                foreach (var car in filteredCars)
                {
                    Console.WriteLine($"Samochód: {car.Make}, {car.Model}, {car.Year}");
                }

                var table = CreateCarTable();

                foreach (var car in filteredCars)
                {
                    table.AddRow(car.Id.ToString(), car.Make, car.Model, car.Year.ToString(), car.Engine.ToString(),
                                  car.HorsePower.ToString(),car.Mileage.ToString(), car.Body, car.Color, car.Price.ToString());
                }

                RenderTable(table);
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
                                    "Minimalna cena", "Maksymalna cena", "Wyczyść wszystkie filtry", "Powrót"));

                switch (choice)
                {
                    case "Marka":
                        var safeMakes = makes.Select(m => Markup.Escape(m)).ToList();
                        make1 = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Wybierz markę:")
                                .AddChoices(safeMakes)
                                .PageSize(10)
                        );

                        if (make1 == "[[Wyczyść filtr]]")
                        {
                            make1 = null;
                        }
                        break;

                    case "Nadwozie":
                        var safeBodies = bodies.Select(b => Markup.Escape(b)).ToList();
                        body1 = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Wybierz nadwozie:")
                                .AddChoices(safeBodies)
                                .PageSize(10)
                        );
                        if (body1 == "[[Wyczyść filtr]]") body1 = null;
                        break;

                    case "Kolor":
                        var safeColors = colors.Select(c => Markup.Escape(c)).ToList();
                        color1 = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("Wybierz kolor:")
                                .AddChoices(safeColors)
                                .PageSize(10)
                        );
                        if (color1 == "[[Wyczyść filtr]]") color1 = null;
                        break;

                    case "Minimalny rocznik":
                        minYear1 = AnsiConsole.Ask<int?>("Podaj minimalny rocznik [gray](lub zostaw puste):[/]", null);
                        break;

                    case "Maksymalny rocznik":
                        maxYear1 = AnsiConsole.Ask<int?>("Podaj maksymalny rocznik [gray](lub zostaw puste):[/]", null);
                        break;

                    case "Minimalny przebieg":
                        minMileage1 = AnsiConsole.Ask<decimal?>("Podaj minimalny przebieg [gray](lub zostaw puste):[/]", null);
                        break;

                    case "Maksymalny przebieg":
                        maxMileage1 = AnsiConsole.Ask<decimal?>("Podaj maksymalny przebieg [gray](lub zostaw puste):[/]", null);
                        break;

                    case "Minimalna cena":
                        minPrice1 = AnsiConsole.Ask<decimal?>("Podaj minimalną cenę [gray](lub zostaw puste):[/]", null);
                        break;

                    case "Maksymalna cena":
                        maxPrice1 = AnsiConsole.Ask<decimal?>("Podaj maksymalną cenę [gray](lub zostaw puste):[/]", null);
                        break;
                    
                    case "Wyczyść wszystkie filtry":
                        make1 = null;
                        body1 = null;
                        color1 = null;
                        minYear1 = null;
                        maxYear1 = null;
                        minMileage1 = null;
                        maxMileage1 = null;
                        minPrice1 = null;
                        maxPrice1 = null;
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

        // Nowa metoda renderująca tabelę
        private void RenderTable(Table table)
        {
            //AnsiConsole.Clear(); // Czyść ekran konsoli
            AnsiConsole.MarkupLine("[green]Dynamiczna tabela wyników wyszukiwania:[/]");
            AnsiConsole.Render(table);
        }
    }
}
