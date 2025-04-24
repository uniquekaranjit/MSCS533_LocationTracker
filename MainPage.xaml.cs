using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Dispatching;
using MSCS533_LocationTracker.Models;
using MSCS533_LocationTracker.Services;
using Microsoft.Maui.Controls.Maps;  // Add this for MapPolygon

namespace MSCS533_LocationTracker;

public partial class MainPage : ContentPage
{
    private readonly LocationDatabase _locationDb;
    private IGeolocation geolocation;
    private IDispatcherTimer timer;  // Changed from ITimer to IDispatcherTimer
    private bool isTracking = false;

    public MainPage(LocationDatabase locationDb)
    {
        InitializeComponent();
        _locationDb = locationDb;
        geolocation = Geolocation.Default;
    }

    private async void OnStartTrackingClicked(object sender, EventArgs e)
    {
        if (!isTracking)
        {
            // Start tracking
            isTracking = true;
            ((Button)sender).Text = "Stop Tracking";
            await StartLocationTracking();
        }
        else
        {
            // Stop tracking
            isTracking = false;
            ((Button)sender).Text = "Start Tracking";
            StopLocationTracking();
        }
    }

    private async Task StartLocationTracking()
    {
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
            return;

        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(10);
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void StopLocationTracking()
    {
        timer?.Stop();
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
        try
        {
            var location = await geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Best,
                Timeout = TimeSpan.FromSeconds(5)
            });

            if (location != null)
            {
                var userLoc = new UserLocation
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Timestamp = DateTime.UtcNow
                };

                await _locationDb.SaveLocationAsync(userLoc);
                AddHeatPoint(userLoc);
                await UpdateMapLocation(location);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task UpdateMapLocation(Location location)
    {
        Map.MoveToRegion(new MapSpan(
            new Location(location.Latitude, location.Longitude),
            0.01,
            0.01
        ));
    }

    private void AddHeatPoint(UserLocation loc)
    {
        // Create a circle to represent the heat point
        var circle = new Circle
        {
            Center = new Location(loc.Latitude, loc.Longitude),
            Radius = new Distance(50), // 50 meters radius
            StrokeColor = Colors.Red,
            StrokeWidth = 2,
            FillColor = Color.FromRgba(255, 0, 0, 0.3)
        };

        Map.MapElements.Add(circle);
    }
}
