using System.Collections.Generic;
using System.Linq;

namespace DakarRally.Models.Vehicles
{
    public class VehicleType
    {
        public int Type { get; set; }
        public int? Subtype { get; set; }
        public Dictionary<string, int> Subtypes { get; set; }

        public VehicleType(int type, int? subtype)
        {
            Type = type;
            Subtype = subtype;
            Subtypes = new Dictionary<string, int>();
        }

        public VehicleType(int type, Dictionary<string, int> subtypes)
        {
            Type = type;
            Subtypes = subtypes;
        }

        private const int CarType = 1;
        private const int TruckType = 2;
        private const int MotorbikeType = 3;
        private const string CarTypeName = "car";
        private const string TruckTypeName = "truck";
        private const string MotorbikeTypeName = "motorbike";
        private const int SportCarSubtype = 11;
        private const int TerrainCarSubtype = 12;
        private const int CrossMotorbikeSubtype = 13;
        private const int SportMotorbikeSubtype = 14;
        private static List<string> TypeNames = new List<string>() { CarTypeName, TruckTypeName, MotorbikeTypeName };

        public static VehicleType Car = new VehicleType(CarType, new Dictionary<string, int>() { { "sport", SportCarSubtype }, { "terrain", TerrainCarSubtype } });
        public static VehicleType Truck = new VehicleType(TruckType, new Dictionary<string, int>());
        public static VehicleType Motorbike = new VehicleType(MotorbikeType, new Dictionary<string, int>() { { "cross", CrossMotorbikeSubtype }, { "sport", SportMotorbikeSubtype } });

        public static VehicleType GetVehicleType(string type, string subtype)
        {
            subtype = subtype.ToLower();
            switch (type.ToLower())
            {
                case CarTypeName:
                    if (Car.Subtypes.TryGetValue(subtype, out int carSubtype))
                    {
                        return new VehicleType(CarType, carSubtype);
                    }
                    return null;
                case TruckTypeName:
                    if (string.IsNullOrEmpty(subtype))
                    {
                        return new VehicleType(TruckType, subtype: null);
                    }
                    return null;
                case MotorbikeTypeName:
                    if (Motorbike.Subtypes.TryGetValue(subtype, out int motorbikeSubtype))
                    {
                        return new VehicleType(MotorbikeType, motorbikeSubtype);
                    }
                    return null;
                default:
                    return null;
            }
        }

        public static bool ValidateType(string type)
        {
            return TypeNames.Contains(type.ToLower());
        }

        public static bool ValidateSubType(string type, string subtype)
        {
            subtype = subtype?.ToLower();
            switch (type.ToLower())
            {
                case CarTypeName:
                    return Car.Subtypes.ContainsKey(subtype);
                case TruckTypeName:
                    return string.IsNullOrEmpty(subtype);
                case MotorbikeTypeName:
                    return Motorbike.Subtypes.ContainsKey(subtype);
                default:
                    return false;
            }
        }
    }
}
