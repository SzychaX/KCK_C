using KCK_APP.Models;
using KCK_APP.Services;
using System.Collections.Generic;

namespace KCK_APP.Controllers
{
    public class ReservationController
    {
        private readonly DatabaseService _databaseService;

        public ReservationController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Dodaj rezerwację
        public void AddReservation(Reservation reservation)
        {
            DatabaseService.AddReservation(reservation);
        }

        // Pobierz wszystkie rezerwacje
        public List<Reservation> GetAllReservations()
        {
            return _databaseService.GetAllReservations();
        }

        // Pobierz rezerwację po ID
        public Reservation? GetReservationById(long id)
        {
            return _databaseService.GetReservationById(id);
        }

        // Pobierz rezerwacje użytkownika
        public List<Reservation> GetReservationsByUserId(long userId)
        {
            return _databaseService.GetReservationsByUserId(userId);
        }

        // Pobierz rezerwacje dla auta
        public List<Reservation> GetReservationsByCarId(long carId)
        {
            return _databaseService.GetReservationsByCarId(carId);
        }

        // Usuń rezerwację
        public void DeleteReservation(long id)
        {
            _databaseService.DeleteReservation(id);
        }
    }
}