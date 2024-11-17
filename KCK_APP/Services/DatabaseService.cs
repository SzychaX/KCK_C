using Npgsql;
using KCK_APP.Models;
using System;
using System.Collections.Generic;
using System.Data;

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
                    Year INTEGER,
                    Mileage DECIMAL,
                    Engine DECIMAL,
                    HorsePower INTEGER,
                    Body VARCHAR(50),
                    COLOR VARCHAR(50),
                    Price DECIMAL
                );", conn);
            cmd.ExecuteNonQuery();
        }

        public void AddCarToDatabase(Car car)
        {
            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO Cars (Make, Model, Year, Mileage, Engine, HorsePower, Body, Color, Price)
                VALUES (@make, @model, @year, @mileage, @engine, @horsePower, @body, @color, @price)", conn);
            cmd.Parameters.AddWithValue("make", car.Make);
            cmd.Parameters.AddWithValue("model", car.Model);
            cmd.Parameters.AddWithValue("year", car.Year);
            cmd.Parameters.AddWithValue("mileage", car.Mileage);
            cmd.Parameters.AddWithValue("engine", car.Engine);
            cmd.Parameters.AddWithValue("horsePower", car.HorsePower);
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
                    Id = reader.GetInt64(0),        // Zmieniłem na `GetInt64` dla Id, jeśli to BigInt
                    Make = reader.GetString(1),
                    Model = reader.GetString(2),
                    Year = reader.GetInt32(3),      // Zmieniłem na `GetInt32` dla Year
                    Mileage = reader.GetDecimal(4),
                    Engine = reader.GetDecimal(5),
                    HorsePower = reader.GetInt32(6),
                    Body = reader.GetString(7),
                    Color = reader.GetString(8),
                    Price = reader.GetDecimal(9)
                });
            }

            return cars;
        }
        
        public List<Car> SearchCars(string make, decimal? maxMileage, int? minHorsePower)
        {
            var cars = new List<Car>();

            // Budujemy zapytanie dynamicznie w zależności od tego, które parametry są dostępne
            var query = "SELECT * FROM Cars WHERE 1=1"; // Początkowy warunek, który zawsze jest prawdziwy

            if (!string.IsNullOrEmpty(make)) // Jeżeli marka nie jest pusta
            {
                query += " AND Make = @make";
            }

            if (maxMileage.HasValue) // Jeżeli maksymalny przebieg jest podany
            {
                query += " AND Mileage <= @maxMileage";
            }

            if (minHorsePower.HasValue) // Jeżeli minimalna moc jest podana
            {
                query += " AND HorsePower >= @minHorsePower";
            }

            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(query, conn);

            // Dodajemy tylko te parametry, które zostały przekazane
            if (!string.IsNullOrEmpty(make))
            {
                cmd.Parameters.AddWithValue("make", make);
            }

            if (maxMileage.HasValue)
            {
                cmd.Parameters.AddWithValue("maxMileage", maxMileage);
            }

            if (minHorsePower.HasValue)
            {
                cmd.Parameters.AddWithValue("minHorsePower", minHorsePower);
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cars.Add(new Car
                {
                    Id = reader.GetInt64(0),        // Zmieniłem na `GetInt64` dla Id, jeśli to BigInt
                    Make = reader.GetString(1),
                    Model = reader.GetString(2),
                    Year = reader.GetInt32(3),      // Zmieniłem na `GetInt32` dla Year
                    Mileage = reader.GetDecimal(4),
                    Engine = reader.GetDecimal(5),
                    HorsePower = reader.GetInt32(6),
                    Body = reader.GetString(7),
                    Color = reader.GetString(8),
                    Price = reader.GetDecimal(9)
                });
            }

            return cars;
        }
        public List<Car> GetFilteredCars(string make, string body, int? minYear, int? maxYear, decimal? minMileage, decimal? maxMileage, decimal? minPrice, decimal? maxPrice, string color)
{
    var cars = new List<Car>();

    using var connection = new NpgsqlConnection(ConnectionString);
    connection.Open();

    string query = @"
    SELECT * FROM Cars
    WHERE Make = COALESCE(@make, Make)
      AND Body = COALESCE(@body, Body)
      AND Year >= COALESCE(@minYear, Year)
      AND Year <= COALESCE(@maxYear, Year)
      AND Mileage >= COALESCE(@minMileage, Mileage)
      AND Mileage <= COALESCE(@maxMileage, Mileage)
      AND Price >= COALESCE(@minPrice, Price)
      AND Price <= COALESCE(@maxPrice, Price)
      AND Color = COALESCE(@color, Color);
    ";

    using var command = new NpgsqlCommand(query, connection);

    command.Parameters.Add("make", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)make ?? DBNull.Value;
    command.Parameters.Add("body", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)body ?? DBNull.Value;
    command.Parameters.Add("minYear", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)minYear ?? DBNull.Value;
    command.Parameters.Add("maxYear", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)maxYear ?? DBNull.Value;
    command.Parameters.Add("minMileage", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)minMileage ?? DBNull.Value;
    command.Parameters.Add("maxMileage", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)maxMileage ?? DBNull.Value;
    command.Parameters.Add("minPrice", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)minPrice ?? DBNull.Value;
    command.Parameters.Add("maxPrice", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)maxPrice ?? DBNull.Value;
    command.Parameters.Add("color", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)color ?? DBNull.Value;
    

    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        cars.Add(new Car
        {
            Id = reader.GetInt64(0),
            Make = reader.GetString(1),
            Model = reader.GetString(2),
            Year = reader.GetInt32(3),
            Mileage = reader.GetDecimal(4),
            Engine = reader.GetDecimal(5),
            HorsePower = reader.GetInt32(6),
            Body = reader.GetString(7),
            Color = reader.GetString(8),
            Price = reader.GetDecimal(9)
        });
    }

    return cars;
}




        public List<string> GetUniqueMakes()
        {
            var makes = new List<string>();
            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            
            using var cmd = new NpgsqlCommand("SELECT DISTINCT Make From Cars Order by Make", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                makes.Add(reader.GetString(0));
            }
            return makes;
        }
        
        public List<string> GetUniqueBodies()
        {
            var bodies = new List<string>();
            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            
            using var cmd = new NpgsqlCommand("SELECT DISTINCT Body From Cars Order by Body", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                bodies.Add(reader.GetString(0));
            }
            return bodies;
        }
        
        public List<string> GetUniqueColors()
        {
            var colors = new List<string>();
            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            
            using var cmd = new NpgsqlCommand("SELECT DISTINCT Color From Cars Order by Color", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                colors.Add(reader.GetString(0));
            }
            return colors;
        }
        
        public List<Car> GetFilteredCarsPaged(string? make, string? body, int? minYear, int? maxYear, decimal? minMileage, decimal? maxMileage, decimal? minPrice, decimal? maxPrice, string? color, int limit, int offset)
{
    var cars = new List<Car>();

    using var connection = new NpgsqlConnection(ConnectionString);
    connection.Open();

    string query = @"
    SELECT * FROM Cars
    WHERE (@make IS NULL OR Make = @make)
      AND (@body IS NULL OR Body = @body)
      AND (@minYear IS NULL OR Year >= @minYear)
      AND (@maxYear IS NULL OR Year <= @maxYear)
      AND (@minMileage IS NULL OR Mileage >= @minMileage)
      AND (@maxMileage IS NULL OR Mileage <= @maxMileage)
      AND (@minPrice IS NULL OR Price >= @minPrice)
      AND (@maxPrice IS NULL OR Price <= @maxPrice)
      AND (@color IS NULL OR Color = @color)
    ORDER BY Id
    LIMIT @limit OFFSET @offset";

    using var command = new NpgsqlCommand(query, connection);

    command.Parameters.Add("make", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)make ?? DBNull.Value;
    command.Parameters.Add("body", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)body ?? DBNull.Value;
    command.Parameters.Add("minYear", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)minYear ?? DBNull.Value;
    command.Parameters.Add("maxYear", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object?)maxYear ?? DBNull.Value;
    command.Parameters.Add("minMileage", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)minMileage ?? DBNull.Value;
    command.Parameters.Add("maxMileage", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)maxMileage ?? DBNull.Value;
    command.Parameters.Add("minPrice", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)minPrice ?? DBNull.Value;
    command.Parameters.Add("maxPrice", NpgsqlTypes.NpgsqlDbType.Numeric).Value = (object?)maxPrice ?? DBNull.Value;
    command.Parameters.Add("color", NpgsqlTypes.NpgsqlDbType.Text).Value = (object?)color ?? DBNull.Value;
    command.Parameters.Add("limit", NpgsqlTypes.NpgsqlDbType.Integer).Value = limit;
    command.Parameters.Add("offset", NpgsqlTypes.NpgsqlDbType.Integer).Value = offset;

    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        cars.Add(new Car
        {
            Id = reader.GetInt64(0),
            Make = reader.GetString(1),
            Model = reader.GetString(2),
            Year = reader.GetInt32(3),
            Mileage = reader.GetDecimal(4),
            Engine = reader.GetDecimal(5),
            HorsePower = reader.GetInt32(6),
            Body = reader.GetString(7),
            Color = reader.GetString(8),
            Price = reader.GetDecimal(9)
        });
    }

    return cars;
}



    }
}     