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
        
        public List<Car> SearchCars(string make, decimal? maxMileage, int? minHorsePower)
        {
            return _databaseService.SearchCars(make, maxMileage, minHorsePower);
        }
        
        public List<Car> GetFilteredCars(string make, string model, int? minYear, int? maxYear, decimal? minMileage, decimal? maxMileage, decimal? minPrice, decimal? maxPrice, string color)
        {
            return _databaseService.GetFilteredCars(make, model, minYear, maxYear, minMileage, maxMileage, minPrice, maxPrice, color);
        }

        public List<string> GetUniqueMakes()
        {
            return _databaseService.GetUniqueMakes();
        }

        public List<string> GetUniqueBodies()
        {
            return _databaseService.GetUniqueBodies();
        }

        public List<string> GetUniqueColors()
        {
            return _databaseService.GetUniqueColors();
        }



    }
}