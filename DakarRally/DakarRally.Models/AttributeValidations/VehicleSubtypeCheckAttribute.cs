using DakarRally.Models.Vehicles;
using System;
using System.ComponentModel.DataAnnotations;

namespace DakarRally.Models.AttributeValidations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class VehicleSubtypeCheckAttribute : ValidationAttribute
    {
        public string DependentUpon { get; set; }
        public VehicleSubtypeCheckAttribute(string dependentUpon)
        {
            this.DependentUpon = dependentUpon;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var dependentValue = context.ObjectInstance.GetType().GetProperty(DependentUpon).GetValue(context.ObjectInstance, null);
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            if (VehicleType.ValidateSubType(dependentValue.ToString(), value.ToString()))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
        }
    }
}
