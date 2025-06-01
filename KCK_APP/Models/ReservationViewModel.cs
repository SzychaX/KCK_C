namespace KCK_APP.Models
{
    public class ReservationViewModel
    {
        public long Id { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string Username { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}