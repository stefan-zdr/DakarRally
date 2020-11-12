using DakarRally.Models.AttributeValidations;
using System;
using System.ComponentModel.DataAnnotations;

namespace DakarRally.Models.Vehicles
{
    public class Vehicle
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string TeamName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Model { get; set; }
        [Required]
        public DateTime ManufacturingDate { get; set; }
        /// <summary>
        /// Vehicle type: Car, Truck, Motorbike
        /// </summary>
        [VehicleTypeCheck]
        [Required]
        [MaxLength(100)]
        public string VehicleType { get; set; }
        /// <summary>
        /// Vehicle subtype:  Sport, Terrain, Cross
        /// </summary>
        [VehicleSubtypeCheck("VehicleType")]
        [MaxLength(100)]
        public string VehicleSubtype { get; set; }
    }
}
