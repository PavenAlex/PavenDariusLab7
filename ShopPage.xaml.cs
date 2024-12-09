using Microsoft.Maui.Devices.Sensors;
using PavenDariusLab7.Models;
using Plugin.LocalNotification;

namespace PavenDariusLab7;

public partial class ShopPage : ContentPage
{
    public ShopPage()
    {
        InitializeComponent();
    }
    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        await App.Database.SaveShopAsync(shop);
        await Navigation.PopAsync();
    }

    async void OnShowMapButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        var address = shop.Adress;
        var locations = await Geocoding.GetLocationsAsync(address);

        var options = new MapLaunchOptions
        {
            Name = "Magazinul meu preferat"
        };
        var shoplocation = locations?.FirstOrDefault();
        /* var shoplocation= new Location(46.7492379, 23.5745597);//pentru
        Windows Machine */

        var myLocation = await Geolocation.GetLocationAsync();
        /* var myLocation = new Location(46.7731796289, 23.6213886738);
       //pentru Windows Machine */
        var distance = myLocation.CalculateDistance(shoplocation, DistanceUnits.Kilometers);
        if (distance < 5)
        {
            var request = new NotificationRequest
            {
                Title = "Ai de facut cumparaturi in apropiere!",
                Description = address,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1)
                }
            };
            LocalNotificationCenter.Current.Show(request);
        }

        await Map.OpenAsync(shoplocation, options);
    }
    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        if (shop == null)
        {
            await DisplayAlert("Eroare", "Nu exista magazin selectat pentru stergere.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmare", "Sigur doriti sa stergeti magazinul {shop.ShopName}?", "Da", "Nu");
        if (confirm)
        {
            await App.Database.DeleteShopAsync(shop);
            await Navigation.PopAsync();
        }
    }
}