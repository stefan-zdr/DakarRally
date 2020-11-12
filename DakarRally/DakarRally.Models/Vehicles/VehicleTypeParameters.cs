using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DakarRally.Models.Vehicles
{
    public class VehicleTypeParameters
    {
        public int VehicleTypeId { get; set; }
        public int TopSpeed { get; set; }
        public int LightMalfunctionTime { get; set; }
        public decimal LightMalfunctionPercentage { get; set; }
        public decimal HeavyMalfunctionPercentage { get; set; }
    }
}
