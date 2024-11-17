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
                var title = new FigletText("Car Catalog")
                    .Color(Color.Green);
                AnsiConsole.Write(new Panel(title)
                    .Expand()
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(new Style(Color.Green)));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Co chcesz zrobić?[/]")
                        .AddChoices(
                            "[green]+ Dodaj samochód[/]",
                            "[yellow]> Pokaż katalog[/]",
                            "[cyan]> Wyszukiwanie[/]",
                            "[red]x Zakończ[/]")
                        .HighlightStyle(new Style(foreground: Color.Aqua, decoration: Decoration.Bold))
                );

                switch (choice)
                {
                    case "[blue]+ Dodaj samochód[/]":
                        AddCar();
                        break;
                    case "[yellow]> Pokaż katalog[/]":
                        ShowCarCatalog();
                        break;
                    case "[cyan]> Wyszukiwanie[/]":
                        SearchCarsMenuLive();
                        break;
                    case "[red]x Zakończ[/]":
                        AnsiConsole.MarkupLine("[red]Do zobaczenia![/]");
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
    string make = null;
    string body = null;
    string color = null;
    int? minYear = null;
    int? maxYear = null;
    decimal? minMileage = null;
    decimal? maxMileage = null;
    decimal? minPrice = null;
    decimal? maxPrice = null;

    const int pageSize = 10;
    int currentPage = 1;

    // Pobierz unikalne wartości do menu
    var makes = _carController.GetUniqueMakes();
    var bodies = _carController.GetUniqueBodies();
    var colors = _carController.GetUniqueColors();
    
    makes.Add("[Wyczyść filtr]");
    bodies.Add("[Wyczyść filtr]");
    colors.Add("[Wyczyść filtr]");

    while (true)
    {
        Console.Clear();

        // Aktualizuj wyniki wyszukiwania z paginacją
        var filteredCars = _carController.GetFilteredCarsPaged(
            make, body, minYear, maxYear, minMileage, maxMileage, minPrice, maxPrice, color, currentPage, pageSize);

        var table = CreateCarTable();

        if (filteredCars.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Brak wyników wyszukiwania dla podanych kryteriów.[/]");
        }
        else
        {
            foreach (var car in filteredCars)
            {
                table.AddRow(car.Id.ToString(), car.Make, car.Model, car.Year.ToString(), car.Engine.ToString(),
                             car.HorsePower.ToString(), car.Mileage.ToString(), car.Body, car.Color, car.Price.ToString());
            }
            RenderTable(table);
        }

        // Nawigacja w menu wyszukiwania i stronicowania
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Co chcesz zrobić?[/]")
                .AddChoices(
                    "Zmiana kryteriów",
                    "Poprzednia strona",
                    "Następna strona",
                    "Powrót")
        );

        switch (choice)
        {
            case "Zmiana kryteriów":
                // Wywołanie pełnego menu zmiany kryteriów
                UpdateFilters(ref make, ref body, ref color, ref minYear, ref maxYear, ref minMileage, ref maxMileage, ref minPrice, ref maxPrice, makes, bodies, colors);
                currentPage = 1; // Zresetuj stronę po zmianie kryteriów
                break;

            case "Poprzednia strona":
                if (currentPage > 1)
                    currentPage--;
                else
                    AnsiConsole.MarkupLine("[yellow]To jest pierwsza strona![/]");
                break;

            case "Następna strona":
                currentPage++;
                break;

            case "Powrót":
                AnsiConsole.Clear();
                return;
        }
    }
}
        
        
        private void UpdateFilters(
    ref string? make, ref string? body, ref string? color, 
    ref int? minYear, ref int? maxYear, 
    ref decimal? minMileage, ref decimal? maxMileage, 
    ref decimal? minPrice, ref decimal? maxPrice, 
    List<string> makes, List<string> bodies, List<string> colors)
{
    while (true)
    {
        // Przygotowanie dynamicznych etykiet z wybranymi wartościami
        string makeLabel = $"Marka ({(make ?? "dowolna")})";
        string bodyLabel = $"Nadwozie ({(body ?? "dowolne")})";
        string colorLabel = $"Kolor ({(color ?? "dowolny")})";
        string minYearLabel = $"Minimalny rocznik ({(minYear?.ToString() ?? "dowolny")})";
        string maxYearLabel = $"Maksymalny rocznik ({(maxYear?.ToString() ?? "dowolny")})";
        string minMileageLabel = $"Minimalny przebieg ({(minMileage?.ToString() ?? "dowolny")})";
        string maxMileageLabel = $"Maksymalny przebieg ({(maxMileage?.ToString() ?? "dowolny")})";
        string minPriceLabel = $"Minimalna cena ({(minPrice?.ToString() ?? "dowolna")})";
        string maxPriceLabel = $"Maksymalna cena ({(maxPrice?.ToString() ?? "dowolna")})";

        // Wyświetlenie dynamicznego menu z aktualnymi wartościami filtrów
        var filterChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Które kryterium chcesz zmienić?[/]")
                .AddChoices(
                    makeLabel, bodyLabel, colorLabel, 
                    minYearLabel, maxYearLabel, 
                    minMileageLabel, maxMileageLabel, 
                    minPriceLabel, maxPriceLabel,
                    "Wyczyść filtry",
                    "Zakończ zmianę kryteriów")
                .HighlightStyle(new Style(foreground: Color.Aqua, decoration: Decoration.Bold))
        );

        // Obsługa wyboru kryterium
        switch (filterChoice)
        {
            case var choice when choice == makeLabel:
                make = ChangeFilter("Marka", makes, make);
                break;

            case var choice when choice == bodyLabel:
                body = ChangeFilter("Nadwozie", bodies, body);
                break;

            case var choice when choice == colorLabel:
                color = ChangeFilter("Kolor", colors, color);
                break;

            case var choice when choice == minYearLabel:
                minYear = AnsiConsole.Ask<int?>("Podaj minimalny rocznik [gray](lub zostaw puste):[/]", null);
                break;

            case var choice when choice == maxYearLabel:
                maxYear = AnsiConsole.Ask<int?>("Podaj maksymalny rocznik [gray](lub zostaw puste):[/]", null);
                break;

            case var choice when choice == minMileageLabel:
                minMileage = AnsiConsole.Ask<decimal?>("Podaj minimalny przebieg [gray](lub zostaw puste):[/]", null);
                break;

            case var choice when choice == maxMileageLabel:
                maxMileage = AnsiConsole.Ask<decimal?>("Podaj maksymalny przebieg [gray](lub zostaw puste):[/]", null);
                break;

            case var choice when choice == minPriceLabel:
                minPrice = AnsiConsole.Ask<decimal?>("Podaj minimalną cenę [gray](lub zostaw puste):[/]", null);
                break;

            case var choice when choice == maxPriceLabel:
                maxPrice = AnsiConsole.Ask<decimal?>("Podaj maksymalną cenę [gray](lub zostaw puste):[/]", null);
                break;
            
            case "Wyczyść filtry":
                make = null;
                body = null;
                color = null;
                minYear = null;
                maxYear = null;
                minMileage = null;
                maxMileage = null;
                minPrice = null;
                maxPrice = null;
                break;

            case "Zakończ zmianę kryteriów":
                return;
        }
    }
}



        private string? ChangeFilter(string title, List<string> options, string? currentValue)
        {
            var safeOptions = options.Select(Markup.Escape).ToList();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Wybierz {title}:")
                    .AddChoices(safeOptions)
                    .PageSize(10)
            );

            return choice == "[[Wyczyść filtr]]" ? null : choice;
        }




        public Table CreateCarTable()
        {
            var carTable = new Table();
            carTable.AddColumn("Id");
            carTable.AddColumn("Marka");
            carTable.AddColumn("Model");
            carTable.AddColumn("Rocznik");
            carTable.AddColumn("Silnik (w cm3)");
            carTable.AddColumn("Moc (w KM)");
            carTable.AddColumn("Przebieg (w km)");
            carTable.AddColumn("Nadwozie");
            carTable.AddColumn("Kolor");
            carTable.AddColumn("Cena (w PLN)");

            return carTable;
        }

        // Nowa metoda renderująca tabelę
        private void RenderTable(Table table)
        {
            AnsiConsole.Clear(); // Czyść ekran konsoli
            AnsiConsole.MarkupLine("[green]Dynamiczna tabela wyników wyszukiwania:[/]");
            AnsiConsole.Render(table);
        }
    }
    
    
}
