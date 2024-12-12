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
            // Ładowanie obrazu samochodu
            CarImage.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri(_car.ImageUrl));
            
            // Formatowanie opisu samochodu, w tym przypadku użycie HTML (np. pogrubienie)
            CarDescription.Inlines.Clear();
            CarDescription.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run($"Marka: {_car.Make}\n")));
            CarDescription.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run($"Model: {_car.Model}\n")));
            CarDescription.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run($"Rok: {_car.Year}\n")));
            CarDescription.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run($"Przebieg: {_car.Mileage} km\n")));
            CarDescription.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run($"Cena: {_car.Price} PLN\n")));
            CarDescription.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run($"Pojemność silnika: {_car.Engine} cm³\n")));
            CarDescription.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run($"Moc: {_car.HorsePower} KM\n")));
            CarDescription.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run($"Rodzaj nadwozia: {_car.Body}\n")));
            CarDescription.Inlines.Add(new System.Windows.Documents.Bold(new System.Windows.Documents.Run($"Kolor: {_car.Color}\n")));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainContent.Content = new SearchCarsView();
        }
    }
}
