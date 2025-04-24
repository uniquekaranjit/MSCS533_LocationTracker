using SQLite;

namespace MSCS533_LocationTracker.Models
{
    public class UserLocation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}