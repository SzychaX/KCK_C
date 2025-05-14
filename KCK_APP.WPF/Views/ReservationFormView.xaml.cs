using System;
using System.Windows;
using System.Windows.Controls;
using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.WPF.Views;

public partial class ReservationFormView : UserControl
{
    private readonly long _carId;
    private readonly long _userId;
    private readonly Action _onReservationSuccess;

    public ReservationFormView(long carId, long userId, Action onReservationSuccess)
    {
        InitializeComponent();
        _carId = carId;
        _userId = userId;
        _onReservationSuccess = onReservationSuccess;
    }

    private void ReserveButton_Click(object sender, RoutedEventArgs e)
    {
        var startDate = StartDatePicker.SelectedDate;
        var endDate = EndDatePicker.SelectedDate;

        if (startDate == null || endDate == null)
        {
            MessageBox.Show("Wybierz daty!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var reservation = new Reservation
        {
            car_id = _carId,
            user_id = _userId,
            start_date = startDate.Value,
            end_date = endDate.Value,
            status = "Aktywna"
        };

        DatabaseService.AddReservation(reservation);

        MessageBox.Show("Rezerwacja została zapisana!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

        _onReservationSuccess?.Invoke();
    }
}