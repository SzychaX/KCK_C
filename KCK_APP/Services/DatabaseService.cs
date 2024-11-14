using Npgsql;
using KCK_APP.Models;
using System;
using System.Collections.Generic;

namespace KCK_APP.Services
{
    public class DatabaseService
    {
        private const string ConnectionString = "Host=localhost;Port=5434;Username=user;Password=password;Database=car_catalog";

        public void CreateTable()
        {
            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS Cars (
                    Id SERIAL PRIMARY KEY,
                    Make VARCHAR(50),
                    Model VARCHAR(50),
                    Year INT,
                    Mileage DECIMAL,
                    Engine DECIMAL,
                    Body VARCHAR(50),
                    COLOR VARCHAR(50),
                    Price DECIMAL
                );", conn);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Tabela Cars została utworzona lub już istnieje.");
        }

        public void AddCarToDatabase(Car car)
        {
            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO Cars (Make, Model, Year, Mileage, Engine, Body, Color, Price)
                VALUES (@make, @model, @year, @mileage, @engine, @body, @color, @price)", conn);
            cmd.Parameters.AddWithValue("make", car.Make);
            cmd.Parameters.AddWithValue("model", car.Model);
            cmd.Parameters.AddWithValue("year", car.Year);
            cmd.Parameters.AddWithValue("mileage", car.Mileage);
            cmd.Parameters.AddWithValue("engine", car.Engine);
            cmd.Parameters.AddWithValue("body", car.Body);
            cmd.Parameters.AddWithValue("color", car.Color);
            cmd.Parameters.AddWithValue("price", car.Price);
            cmd.ExecuteNonQuery();
        }

        public List<Car> GetAllCars()
        {
            var cars = new List<Car>();

            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT * FROM Cars;", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cars.Add(new Car
                {
                    Id = reader.GetInt64(0),
                    Make = reader.GetString(1),
                    Model = reader.GetString(2),
                    Year = reader.GetInt32(3),
                    Mileage = reader.GetDecimal(5),
                    Engine = reader.GetDecimal(6),
                    Body = reader.GetString(7),
                    Color = reader.GetString(8),
                    Price = reader.GetDecimal(9)
                });
            }

            return cars;
        }
    }
}     