using DakarRally.Models.Vehicles;
using System;
using System.ComponentModel.DataAnnotations;

namespace DakarRally.Models.AttributeValidations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class VehicleTypeCheckAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }
            return VehicleType.ValidateType(value.ToString());
        }
    }
}
