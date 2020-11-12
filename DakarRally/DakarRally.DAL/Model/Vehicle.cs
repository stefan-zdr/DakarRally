//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DakarRally.DAL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Vehicle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vehicle()
        {
            this.Malfunctions = new HashSet<Malfunction>();
            this.RaceVehicles = new HashSet<RaceVehicle>();
        }
    
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Model { get; set; }
        public System.DateTime ManufacturingDate { get; set; }
        public int VehicleTypeId { get; set; }
        public Nullable<int> VehicleSubtypeId { get; set; }
    
        public virtual VehicleType VehicleSubtype { get; set; }
        public virtual VehicleType VehicleType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Malfunction> Malfunctions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RaceVehicle> RaceVehicles { get; set; }
    }
}