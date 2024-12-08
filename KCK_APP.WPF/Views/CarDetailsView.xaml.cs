using System.Windows;
using System.Windows.Controls;
using KCK_APP.Models;

namespace KCK_APP.WPF.Views
{
    public partial class CarDetailsView : UserControl
    {
        private Car _car;

        public CarDetailsView(Car car)
        {
            InitializeComponent();
            _car = car;
            LoadCarDetails();
        }

        private void LoadCarDetails()
        {
            CarImage.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri(_car.ImageUrl));
            CarDescription.Text = 
                $"Marka: {_car.Make}\n" +
                $"Model: {_car.Model}\n" +
                $"Rok: {_car.Year}\n" +
                $"Przebieg: {_car.Mileage} km\n" +
                $"Cena: {_car.Price} PLN\n" +
                $"Pojemność silnika: {_car.Engine} cm³\n" +
                $"Moc: {_car.HorsePower} KM\n" +
                $"Rodzaj nadwozia: {_car.Body}\n" +
                $"Kolor: {_car.Color}\n";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainContent.Content = new SearchCarsView();
        }
    }
}