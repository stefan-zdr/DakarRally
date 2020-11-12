using System;

namespace DakarRally.Models.Vehicles
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Model { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public int VehicleTypeId { get; set; }
        public string VehicleTypeName { get; set; }
        public int? VehicleSubtypeId { get; set; }
        public string VehicleSubtypeName { get; set; }
    }
}
