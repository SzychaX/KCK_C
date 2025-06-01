namespace KCK_APP.Models;

public class Reservation
{
    public long _id { get; set; }
    public long car_id { get; set; }
    public long user_id { get; set; }
    public DateTime start_date { get; set; }
    public DateTime end_date { get; set; }
}