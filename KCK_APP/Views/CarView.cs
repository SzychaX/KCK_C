using KCK_APP.Controllers;
using KCK_APP.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;

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
    string model = null;
    int? minYear = null;
    int? maxYear = null;
    decimal? minMileage = null;
    decimal? maxMileage = null;
    decimal? minPrice = null;
    decimal? maxPrice = null;

    // Wyświetlanie dynamicznej tabeli
    void UpdateTable()
    {
        var filteredCars = _carController.GetFilteredCars(make, model, minYear, maxYear, minMileage, maxMileage, minPrice, maxPrice);

        var table = CreateCarTable();

        foreach (var car in filteredCars)
        {
            table.AddRow(car.Id.ToString(), car.Make, car.Model, car.Year.ToString(), car.Engine.ToString(), 
                         car.Mileage.ToString(), car.HorsePower.ToString(), car.Body, car.Color, car.Price.ToString());
        }

        AnsiConsole.Clear(); // Czyści konsolę przed odrysowaniem
        AnsiConsole.MarkupLine("[green]Dynamiczna tabela wyników wyszukiwania:[/]");
        AnsiConsole.Render(table);
    }

    // Formularz do wprowadzania kryteriów
    while (true)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Co chcesz zmienić?")
                .AddChoices("Marka", "Model", "Minimalny rocznik", "Maksymalny rocznik", 
                            "Minimalny przebieg", "Maksymalny przebieg", 
                            "Minimalna cena", "Maksymalna cena", "Powrót"));

        switch (choice)
        {
            case "Marka":
                make = AnsiConsole.Ask<string>("Podaj [blue]markę[/] (lub zostaw puste):", "");
                break;

            case "Model":
                model = AnsiConsole.Ask<string>("Podaj [blue]model[/] (lub zostaw puste):", "");
                break;

            case "Minimalny rocznik":
                minYear = AnsiConsole.Ask<int?>("Podaj [blue]minimalny rocznik[/] (lub zostaw puste):", null);
                break;

            case "Maksymalny rocznik":
                maxYear = AnsiConsole.Ask<int?>("Podaj [blue]maksymalny rocznik[/] (lub zostaw puste):", null);
                break;

            case "Minimalny przebieg":
                minMileage = AnsiConsole.Ask<decimal?>("Podaj [blue]minimalny przebieg[/] (lub zostaw puste):", null);
                break;

            case "Maksymalny przebieg":
                maxMileage = AnsiConsole.Ask<decimal?>("Podaj [blue]maksymalny przebieg[/] (lub zostaw puste):", null);
                break;

            case "Minimalna cena":
                minPrice = AnsiConsole.Ask<decimal?>("Podaj [blue]minimalną cenę[/] (lub zostaw puste):", null);
                break;

            case "Maksymalna cena":
                maxPrice = AnsiConsole.Ask<decimal?>("Podaj [blue]maksymalną cenę[/] (lub zostaw puste):", null);
                break;

            case "Powrót":
                return;
        }

        // Aktualizuj tabelę po każdej zmianie
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