# MSCS533 Location Tracker

This project is a **Location Tracker** application built using .NET MAUI. It provides a map interface with features such as zoom controls, location simulation, and a visually appealing title bar.

## Features

- **Map Integration**: Displays a map with user location tracking.
- **Custom Title Bar**: A purple title bar at the top with the title "MainView."
- **Zoom Controls**: Buttons to zoom in and out of the map.
- **Location Simulation**: Simulates movement along predefined locations.
- **Heatmap Visualization**: Adds blue dots for simulated locations and connects them with a red path.

## Project Structure

- **`MainPage.xaml`**: Defines the UI layout, including the map, title bar, and zoom controls.
- **`MainPage.xaml.cs`**: Contains the logic for location simulation, zoom functionality, and map updates.
- **`AppShell.xaml`**: Configures the navigation structure of the app.
- **`App.xaml.cs`**: Initializes the app and sets the main page.

## How to Run

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/uniquekaranjit/MSCS533_LocationTracker 
   cd MSCS533_LocationTracker
   ```
2. **Build the Project**:
    ```bash 
    dotnet build 
    ```
3. **Run the App**:
    For iOS:
    ```bash
    dotnet run -f net8.0-ios
    ```
    **Note: The project has been setup for iOS 18.3, so please make sure the emulator has iOS18.3 and iPhone 16 Pro, and has been tested on that. 

    For Android:
    ```bash
    dotnet run -f net8.0-android
    ```

