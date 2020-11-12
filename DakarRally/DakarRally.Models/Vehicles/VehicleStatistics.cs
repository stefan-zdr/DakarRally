using System;

namespace DakarRally.Models.Vehicles
{
    public class VehicleStatistics
    {
        public decimal Distance { get; set; }
        public MalfunctionStatus Status { get; set; }
        public DateTime? CurrentMalfunctionStartAt { get; set; }
        public int NumberOfMalfunctions { get; set; }
        public decimal? FinishTime { get; set; }
    }
}
