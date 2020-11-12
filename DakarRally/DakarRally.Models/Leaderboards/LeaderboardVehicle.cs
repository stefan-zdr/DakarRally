using DakarRally.Models.Malfunctions;
using DakarRally.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DakarRally.Models.Leaderboards
{
    public class LeaderboardVehicle
    {
        public int VehicleId { get; set; }
        public int Position { get; set; }
        public string Model { get; set; }
        public string TeamName { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public decimal? FinishTime { get; set; }
        public int TopSpeed { get; set; }
        public MalfunctionStatus MalfunctionStatus { get; set; }
        public List<MalfunctionStats> Malfunctions { get; set; }

        public DateTime? RaceStartAt { get; set; }
        public DateTime LeaderboardForDateTime { get; set; }
        public decimal Distance { get; set; }
    }
}
