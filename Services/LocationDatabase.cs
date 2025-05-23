using SQLite;
using MSCS533_LocationTracker.Models;

namespace MSCS533_LocationTracker.Services
{
    public class LocationDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public LocationDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "locations.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<UserLocation>().Wait();
        }

        public Task<int> SaveLocationAsync(UserLocation location)
        {
            return _database.InsertAsync(location);
        }

        public Task<List<UserLocation>> GetLocationsAsync()
        {
            return _database.Table<UserLocation>().ToListAsync();
        }
    }
}