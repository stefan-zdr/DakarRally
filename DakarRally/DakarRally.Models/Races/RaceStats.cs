using DakarRally.Models.Vehicles;
using System.Collections.Generic;

namespace DakarRally.Models.Races
{
    public class RaceStats
    {
        public RaceStatus Status { get; set; }
        public Dictionary<MalfunctionStatus, int> VehiclesNumberByStatus { get; set; }
        public Dictionary<string, int> VehiclesNumberByType { get; set; }
    }
}
