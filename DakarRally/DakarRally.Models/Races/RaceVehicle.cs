using DakarRally.Models.Malfunctions;
using DakarRally.Models.Vehicles;
using System;
using System.Collections.Generic;

namespace DakarRally.Models.Races
{
    public class RaceVehicle
    {
        public int VehicleId { get; set; }
        public string Model { get; set; }
        public string TeamName { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public List<MalfunctionStats> Malfunctions { get; set; }
        public decimal DistanceInMeters { get; set; }
        public decimal? FinishTime { get; set; }
        public DateTime? EndsRaceAt { get; set; }
        public MalfunctionStatus MalfunctionStatus { get; set; }
        public VehicleTypeParameters VehicleTypeParameters { get; set; }
    }
}
