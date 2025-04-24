using SQLite;
using MSCS533_LocationTracker.Models;

namespace MSCS533_LocationTracker.Services
{
    public class LocationDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public LocationDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<UserLocation>().Wait();
        }

        public Task<int> SaveLocationAsync(UserLocation location) =>
            _database.InsertAsync(location);

        public Task<List<UserLocation>> GetLocationsAsync() =>
            _database.Table<UserLocation>().ToListAsync();
    }
}