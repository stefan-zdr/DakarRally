using System;

namespace DakarRally.Models.Vehicles
{
    public class VehicleStats
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Model { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public MalfunctionStatus Status { get; set; }
        /// <summary>
        /// Distance in meters
        /// </summary>
        public decimal Distance { get; set; }
        /// <summary>
        /// Vehicle type: Car, Truck, Motorbike
        /// </summary>
        public string VehicleType { get; set; }
        /// <summary>
        /// Vehicle subtype:  Sport, Terrain, Cross
        /// </summary>
        public string VehicleSubtype { get; set; }
    }
}
