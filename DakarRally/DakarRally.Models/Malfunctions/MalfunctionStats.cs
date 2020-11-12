using System;

namespace DakarRally.Models.Malfunctions
{
    public class MalfunctionStats
    {
        public short Status { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }
    }
}
