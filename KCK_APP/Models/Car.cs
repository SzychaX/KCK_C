namespace KCK_APP.Models;

public class Car
{
    public long
        Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal Mileage { get; set; }
    public decimal Engine { get; set; }
    public int HorsePower { get; set; }
    public string Body { get; set; }
    public string Color { get; set; }
    public decimal Price { get; set; }
}