using System;

namespace DakarRally.Models.Races
{
    public class Race
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndssAt { get; set; }
        public RaceStatus Status { get; set; }
    }
}