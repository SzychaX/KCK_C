using KCK_APP.Controllers;
using KCK_APP.Models;
using Spectre.Console;

namespace KCK_APP.Views
{
    public class CarView
    {
        private readonly CarController _carController;

        public CarView(CarController carController)
        {
            _carController = carController;
        }

        int consoleWidth = Console.WindowWidth;

        public void DisplayMenu()
        {
            while (true)
            {
                Console.Clear();


                // Tworzenie panelu z obramowaniem dla tytułu
                var titlePanel = new Panel(new FigletText("KOMIS").Centered().Color(Color.Cyan2))
                {
                    Border = BoxBorder.Double,
                    BorderStyle = new Style(foreground: Color.Blue),
                    Padding = new Padding(2, 1),
                    Header = new PanelHeader(" Witamy! ", Justify.Center)
                };

                AnsiConsole.Write(titlePanel);


                // Wyświetlenie menu z wyśrodkowanymi opcjami
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title(CenterText("[Cyan2 bold]MENU[/]", consoleWidth))
                        .AddChoices(new[]
                        {
                            CenterTextForChoices("[green]Szukaj samochodu[/]", consoleWidth),
                            CenterTextForChoices("[green]Zarządzanie samochodami[/]", consoleWidth),
                            CenterTextForChoices("[red]Zakończ[/]", consoleWidth)
                        })
                        .HighlightStyle(new Style(foreground: Color.Cyan1))
                );

                // Obsługa wyboru
                switch (choice.Trim()) // Trim usuwa dodatkowe spacje dodane przez CenterText
                {
                    case "[green]Zarządzanie samochodami[/]":
                        if (AuthenticateUser())
                        {
                            ManageCars();
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[red]Dostęp do zarządzania samochodami został odmówiony.[/]");
                        }

                        break;


                    case "[green]Szukaj samochodu[/]":
                        SearchCarsMenuLive();
                        break;

                    case "[red]Zakończ[/]":
                        Environment.Exit(0);
                        break;
                }
            }
        }
        // Zarządzanie autami
        private void ManageCars()
        {
            while (true)
            {
                Console.Clear();

                // Tworzenie panelu z obramowaniem dla tytułu "Zarządzanie autami"
                var titlePanel = new Panel(new FigletText("ZARZĄDZANIE").Centered().Color(Color.Cyan2))
                {
                    Border = BoxBorder.Double,
                    BorderStyle = new Style(foreground: Color.Blue),
                    Padding = new Padding(2, 1),
                    Header = new PanelHeader(" Opcje zarządzania ", Justify.Center)
                };

                AnsiConsole.Write(titlePanel);

                // Podmenu zarządzania autami
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(
                            CenterTextForChoices("[green]Dodaj samochód[/]", consoleWidth),
                            CenterTextForChoices("[green]Edytuj samochód[/]", consoleWidth),
                            CenterTextForChoices("[green]Usuń samochód[/]", consoleWidth),
                            CenterTextForChoices("[red]Powrót[/]", consoleWidth))
                        .HighlightStyle(new Style(foreground: Color.Cyan1))
                );

                switch (choice.Trim())
                {
                    case "[green]Dodaj samochód[/]":
                        AddCar();
                        break;

                    case "[green]Edytuj samochód[/]":
                        EditCar();
                        break;

                    case "[green]Usuń samochód[/]":
                        DeleteCar();
                        break;

                    case "[red]Powrót[/]":
                        return;
                }
            }
        }


// Funkcja pomocnicza do wyśrodkowywania tekstu w poziomie
        private string CenterText(string text, int consoleWidth)
        {
            int padding = (consoleWidth - StripMarkup(text).Length) / 2;
            return new string(' ', Math.Max(0, padding)) + text;
        }

        private string CenterTextForChoices(string text, int consoleWidth)
        {
            int padding = (consoleWidth - 3 - StripMarkup(text).Length) / 2;
            return new string(' ', Math.Max(0, padding)) + text;
        }

// Funkcja pomocnicza do usuwania znaczników Markdown
        private string StripMarkup(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, @"\[[^\]]*\]", string.Empty);
        }


        public void AddCar()
        {
            var make = AnsiConsole.Ask<string>("Marka pojazdu: ");
            var model = AnsiConsole.Ask<string>("Model pojazdu: ");
            var year = AnsiConsole.Ask<int>("Rocznik samochodu (rok): ");
            var engine = AnsiConsole.Ask<int>("Silnik samochodu (cm3): ");
            var horsepower = AnsiConsole.Ask<int>("Moc samochodu (KM): ");
            var mileage = AnsiConsole.Ask<int>("Przebieg samochodu (km): ");
            var body = AnsiConsole.Ask<string>("Rodzaj nadwozia: ");
            var color = AnsiConsole.Ask<string>("Kolor samochodu: ");
            var price = AnsiConsole.Ask<int>("Cena: ");

            var car = new Car()
            {
                Make = CapitalizeFirstLetter(make),
                Model = model,
                Year = year,
                Engine = engine,
                HorsePower = horsepower,
                Mileage = mileage,
                Body = body,
                Color = CapitalizeFirstLetter(color),
                Price = price
            };

            _carController.AddCar(car);
            AnsiConsole.MarkupLine("[green]Samochod zostal dodany do katalogu![/]");
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
                    make, body, minYear, maxYear, minMileage, maxMileage, minPrice, maxPrice, color, currentPage,
                    pageSize);

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
                            car.HorsePower.ToString(), car.Mileage.ToString(), car.Body, car.Color,
                            car.Price.ToString());
                    }

                    RenderTable(table);
                }

                // Nawigacja w menu wyszukiwania i stronicowania
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title(CenterText("[cyan1]WYSZUKIWANIE[/]", consoleWidth))
                        .AddChoices(
                            CenterTextForChoices("[green]Zmiana kryteriów[/]", consoleWidth),
                            CenterTextForChoices("[yellow]Poprzednia strona[/]", consoleWidth),
                            CenterTextForChoices("[yellow]Następna strona[/]", consoleWidth),
                            CenterTextForChoices("[red]Powrót[/]", consoleWidth))
                        .HighlightStyle(new Style(foreground: Color.Cyan1))
                );

                switch (choice.Trim())
                {
                    case "[green]Zmiana kryteriów[/]":
                        // Wywołanie pełnego menu zmiany kryteriów z bieżącym odświeżaniem
                        UpdateFilters(
                            ref make, ref body, ref color,
                            ref minYear, ref maxYear,
                            ref minMileage, ref maxMileage,
                            ref minPrice, ref maxPrice,
                            makes, bodies, colors,
                            currentPage, pageSize);
                        break;

                    case "[yellow]Poprzednia strona[/]":
                        if (currentPage > 1)
                            currentPage--;
                        else
                            AnsiConsole.MarkupLine("[yellow]To jest pierwsza strona![/]");
                        break;

                    case "[yellow]Następna strona[/]":
                        currentPage++;
                        break;

                    case "[red]Powrót[/]":
                        return;
                }
            }
        }


        private void UpdateFilters(
            ref string? make, ref string? body, ref string? color,
            ref int? minYear, ref int? maxYear,
            ref decimal? minMileage, ref decimal? maxMileage,
            ref decimal? minPrice, ref decimal? maxPrice,
            List<string> makes, List<string> bodies, List<string> colors,
            int currentPage, int pageSize)
        {
            while (true)
            {
                Console.Clear();

                // Wyświetlanie wyników na bieżąco
                var filteredCars = _carController.GetFilteredCarsPaged(
                    make, body, minYear, maxYear, minMileage, maxMileage, minPrice, maxPrice, color, currentPage,
                    pageSize);

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
                            car.HorsePower.ToString(), car.Mileage.ToString(), car.Body, car.Color,
                            car.Price.ToString());
                    }

                    RenderTable(table);
                }

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
                            "[blue]Wyczyść filtry[/]",
                            "[red]Zakończ zmianę kryteriów[/]")
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
                        minMileage = AnsiConsole.Ask<decimal?>("Podaj minimalny przebieg [gray](lub zostaw puste):[/]",
                            null);
                        break;

                    case var choice when choice == maxMileageLabel:
                        maxMileage = AnsiConsole.Ask<decimal?>("Podaj maksymalny przebieg [gray](lub zostaw puste):[/]",
                            null);
                        break;

                    case var choice when choice == minPriceLabel:
                        minPrice = AnsiConsole.Ask<decimal?>("Podaj minimalną cenę [gray](lub zostaw puste):[/]", null);
                        break;

                    case var choice when choice == maxPriceLabel:
                        maxPrice = AnsiConsole.Ask<decimal?>("Podaj maksymalną cenę [gray](lub zostaw puste):[/]",
                            null);
                        break;

                    case "[blue]Wyczyść filtry[/]":
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

                    case "[red]Zakończ zmianę kryteriów[/]":
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
            var carTable = new Table()
            {
                Border = TableBorder.Rounded, // Dodaj bardziej atrakcyjną ramkę
            };

            // Dodaj nagłówki z kolorami
            carTable.AddColumn(new TableColumn("[red]Id[/]").Centered());
            carTable.AddColumn(new TableColumn("[aqua]Marka[/]").Centered());
            carTable.AddColumn(new TableColumn("[aqua]Model[/]").Centered());
            carTable.AddColumn(new TableColumn("[blue]Rocznik[/]").Centered());
            carTable.AddColumn(new TableColumn("[blue]Silnik (w cm3)[/]").Centered());
            carTable.AddColumn(new TableColumn("[blue]Moc (w KM)[/]").Centered());
            carTable.AddColumn(new TableColumn("[blue]Przebieg (w km)[/]").Centered());
            carTable.AddColumn(new TableColumn("[blue]Nadwozie[/]").Centered());
            carTable.AddColumn(new TableColumn("[blue]Kolor[/]").Centered());
            carTable.AddColumn(new TableColumn("[yellow]Cena (w PLN)[/]").Centered());

            return carTable;
        }

        // Nowa metoda renderująca tabelę
        private void RenderTable(Table table)
        {
            AnsiConsole.Clear(); // Czyść ekran konsoli
            AnsiConsole.Render(table);
        }

        public void EditCar()
        {
            Console.Clear();

            // Pobierz ID samochodu do edycji
            var carId = AnsiConsole.Ask<long>("Podaj ID samochodu do edycji:");

            // Pobierz samochód z bazy
            var car = _carController.GetAllCars().FirstOrDefault(c => c.Id == carId);

            if (car == null)
            {
                AnsiConsole.MarkupLine("[red]Nie znaleziono samochodu o podanym ID.[/]");
                return;
            }

            // Wyświetl aktualne dane i pozwól na ich edycję
            car.Make = AnsiConsole.Ask<string>($"Marka samochodu ([gray]{car.Make}[/]):", car.Make);
            car.Model = AnsiConsole.Ask<string>($"Model samochodu ([gray]{car.Model}[/]):", car.Model);
            car.Year = AnsiConsole.Ask<int>($"Rocznik samochodu ([gray]{car.Year}[/]):", car.Year);
            car.Mileage = AnsiConsole.Ask<decimal>($"Przebieg samochodu ([gray]{car.Mileage}[/]):", car.Mileage);
            car.Engine = AnsiConsole.Ask<decimal>($"Pojemność silnika ([gray]{car.Engine}[/]):", car.Engine);
            car.HorsePower = AnsiConsole.Ask<int>($"Moc ([gray]{car.HorsePower}[/]):", car.HorsePower);
            car.Body = AnsiConsole.Ask<string>($"Nadwozie ([gray]{car.Body}[/]):", car.Body);
            car.Color = AnsiConsole.Ask<string>($"Kolor ([gray]{car.Color}[/]):", car.Color);
            car.Price = AnsiConsole.Ask<decimal>($"Cena ([gray]{car.Price}[/]):", car.Price);

            // Aktualizuj samochód w bazie
            _carController.UpdateCar(car);
            AnsiConsole.MarkupLine("[green]Dane samochodu zostały zaktualizowane![/]");
        }

        public string CapitalizeFirstLetter(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }

        private void DeleteCar()
        {
            long id = AnsiConsole.Ask<long>("Podaj [red]ID samochodu[/] do usunięcia: ");
            var car = _carController.GetAllCars().Find(c => c.Id == id);

            if (car == null)
            {
                AnsiConsole.MarkupLine("[red]Nie znaleziono samochodu o podanym ID.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"Usuwasz samochód: [yellow]{car.Make} {car.Model}[/]");
            var confirmation = AnsiConsole.Confirm("Czy na pewno chcesz usunąć ten samochód?");

            if (confirmation)
            {
                // Wywołaj metodę usuwania samochodu w kontrolerze
                _carController.DeleteCar(id);
                AnsiConsole.MarkupLine("[green]Samochód został usunięty![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Operacja anulowana.[/]");
            }
        }

        private bool AuthenticateUser()
        {
            // Przykładowy login i hasło - w rzeczywistej aplikacji te dane powinny być przechowywane w bezpieczny sposób.
            const string validLogin = "admin";
            const string validPassword = "admin";

            // Prośba o login
            var login = AnsiConsole.Prompt(
                new TextPrompt<string>("[green]Podaj login:[/]")
                    .PromptStyle("green")
                    .ValidationErrorMessage("[red]Login nie może być pusty![/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input))
            );

            // Prośba o hasło (hasło nie będzie widoczne w konsoli)
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("[green]Podaj hasło:[/]")
                    .PromptStyle("green")
                    .Secret() // Ukrycie wpisanego hasła
                    .ValidationErrorMessage("[red]Hasło nie może być puste![/]")
                    .Validate(input => !string.IsNullOrWhiteSpace(input))
            );

            // Weryfikacja danych logowania
            if (login == validLogin && password == validPassword)
            {
                AnsiConsole.MarkupLine("[green]Zalogowano pomyślnie![/]");
                return true;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Nieprawidłowy login lub hasło.[/]");
                return false;
            }
        }
    }
}