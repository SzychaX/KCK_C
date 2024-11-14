using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.Controllers
{

    public class CarController
    {
        private readonly DatabaseService _databaseService;

        public CarController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void AddCar(Car car)
        {
            _databaseService.AddCarToDatabase(car);
        }

        public List<Car> GetAllCars()
        {
            return _databaseService.GetAllCars();
        }

    }
}