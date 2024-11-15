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
                        .AddChoices("Dodaj samochod", "Pokaz katalog", "Zakoncz"));

                switch (choice)
                {
                    case "Dodaj samochod":
                        AddCar();
                        break;
                    case "Pokaz katalog":
                        ShowCarCatalog();
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
                var table = new Table();
                table.AddColumn("Id");
                table.AddColumn("Marka");
                table.AddColumn("Model");
                table.AddColumn("Rocznik");
                table.AddColumn("Silnik");
                table.AddColumn("Moc");
                table.AddColumn("Przebieg");
                table.AddColumn("Nadwozie");
                table.AddColumn("Kolor");
                table.AddColumn("Cena");

                foreach (var car in cars)
                {
                    table.AddRow(car.Id.ToString(), car.Make, car.Model, car.Year.ToString(), car.Engine.ToString(), car.HorsePower.ToString(), car.Mileage.ToString(), car.Body, car.Color, car.Price.ToString());
                }
                AnsiConsole.Render(table);
            }
        }
    }
} 