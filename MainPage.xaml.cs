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

    private readonly List<Location> predefinedLocations = new()
    {
	
		new Location(34.061699, -118.308970),
		new Location(34.0617076, -118.3088505),
		new Location(34.0617161, -118.3087310),
		new Location(34.0617247, -118.3086115),
		new Location(34.0617332, -118.3084920),
		new Location(34.0617418, -118.3083725),
		new Location(34.0617503, -118.3082530),
		new Location(34.0617589, -118.3081335),
		new Location(34.0617674, -118.3080140),
	new Location(34.0617760, -118.3078945),
	new Location(34.0618760, -118.3078945),
	new Location(34.0619760, -118.3078945),
	new Location(34.0620760, -118.3078945),
	new Location(34.0621760, -118.3078945),
	new Location(34.0622760, -118.3078945),
	new Location(34.0623760, -118.3078945),
	new Location(34.0624760, -118.3078945),
	new Location(34.0625760, -118.3078945),
	new Location(34.0626760, -118.3078945)



    };

    private int currentLocationIndex = 0;
    private double currentZoomLevel = 1.0; // Default zoom level

    private Polyline pathLine = new Polyline
    {
        StrokeColor = Colors.Red,
        StrokeWidth = 3 // Thickness of the red line
    };

    public MainPage(LocationDatabase locationDb)
    {
        InitializeComponent();
        _locationDb = locationDb;
        geolocation = Geolocation.Default;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartSimulation();
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
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(5); // Update every 5 seconds
        timer.Tick += (sender, e) =>
        {
            SimulateLocation();
        };
        timer.Start();
    }

    private void StopLocationTracking()
    {
        timer?.Stop();
        timer = null;
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
        try
        {
            var location = await geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Best,
                Timeout = TimeSpan.FromSeconds(0.5)
            });

            if (location != null)
            {
                // Save location to the database
                var userLoc = new UserLocation
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Timestamp = DateTime.UtcNow
                };

                await _locationDb.SaveLocationAsync(userLoc);

                // Add a heat point for the current location
                AddHeatPoint(userLoc);

                // Update the map to center on the current location
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Location(location.Latitude, location.Longitude),
                    Distance.FromKilometers(1)));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to get location: {ex.Message}", "OK");
        }
    }

    private void StartSimulation()
    {
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(500); // Update every 5 milliseconds
        timer.Tick += (sender, e) =>
        {
            SimulateLocation();
        };
        timer.Start();
    }

    private void SimulateLocation()
    {
        if (currentLocationIndex >= predefinedLocations.Count)
        {
            // Stop the simulation when all locations are visited
            StopSimulation();
            return;
        }

        var location = predefinedLocations[currentLocationIndex];
        currentLocationIndex++;

        // Simulate saving location to the database
        var userLoc = new UserLocation
        {
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Timestamp = DateTime.UtcNow
        };

        _locationDb.SaveLocationAsync(userLoc).Wait();

        // Add a blue dot for the simulated location
        AddHeatPoint(userLoc);

        // Add only the points along the circumference of the circle to the red line path
        AddCircumferencePoints(location);

        // Add the red line to the map if it's not already added
        if (!Map.MapElements.Contains(pathLine))
        {
            Map.MapElements.Add(pathLine);
        }

        // Update the map to center on the simulated location
        Map.MoveToRegion(MapSpan.FromCenterAndRadius(
            new Location(location.Latitude, location.Longitude),
            Distance.FromKilometers(0.1))); // Smaller radius for close locations
    }

    private void StopSimulation()
    {
        timer?.Stop();
        timer = null;
        DisplayAlert("Simulation Complete", "The simulation has completed all predefined locations.", "OK");
    }

    private void AddHeatPoint(UserLocation loc)
    {
        // Create a circle to represent the blue dot
        var circle = new Circle
        {
            Center = new Location(loc.Latitude, loc.Longitude),
            Radius = new Distance(5), // 5 meters radius for smaller points
            StrokeColor = Colors.Blue,
            StrokeWidth = 1,
            FillColor = Color.FromRgba(0, 0, 255, 0.5) // Semi-transparent blue
        };

        Map.MapElements.Add(circle);
    }

    private async void OnGpsButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var location = await geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Best,
                Timeout = TimeSpan.FromSeconds(10)
            });

            if (location != null)
            {
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Location(location.Latitude, location.Longitude),
                    Distance.FromKilometers(1)));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to get location: {ex.Message}", "OK");
        }
    }

    private void OnZoomInClicked(object sender, EventArgs e)
    {
        currentZoomLevel *= 0.90; // Zoom in by reducing the radius by 5%
        UpdateMapZoom();
    }

    private void OnZoomOutClicked(object sender, EventArgs e)
    {
        currentZoomLevel *= 1.1; // Zoom out by increasing the radius by 5%
        UpdateMapZoom();
    }

    private void UpdateMapZoom()
    {
        if (Map.VisibleRegion != null)
        {
            var center = Map.VisibleRegion.Center;
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(currentZoomLevel)));
        }
    }

    private void AddCircumferencePoints(Location center)
    {
        const int numberOfPoints = 36; // Number of points to approximate the circle (higher = smoother)
        const double radiusInMeters = 5; // Radius of the circle in meters

        for (int i = 0; i < numberOfPoints; i++)
        {
            double angle = 2 * Math.PI * i / numberOfPoints; // Angle in radians
            double offsetX = radiusInMeters * Math.Cos(angle); // X offset
            double offsetY = radiusInMeters * Math.Sin(angle); // Y offset

            // Convert the offsets to latitude and longitude
            double latitudeOffset = offsetY / 111320; // Approx. meters per degree latitude
            double longitudeOffset = offsetX / (111320 * Math.Cos(center.Latitude * Math.PI / 180)); // Adjust for longitude

            var point = new Location(center.Latitude + latitudeOffset, center.Longitude + longitudeOffset);
            pathLine.Geopath.Add(point);
        }
    }
}
