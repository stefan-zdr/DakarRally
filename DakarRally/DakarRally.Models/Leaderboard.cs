using DakarRally.Models.Vehicles;

namespace DakarRally.Models
{
    public class Leaderboard
    {
        public int Position { get; set; }
        public string Model { get; set; }
        public string TeamName { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public MalfunctionStatus MalfunctionStatus { get; set; }
    }
}
