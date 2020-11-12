using DakarRally.Models;
using DakarRally.Models.Races;
using DakarRally.Models.Vehicles;
using System;
using System.Collections.Generic;

namespace DakarRally.Interfaces.Services
{
    public interface ILeaderboardsService
    {
        List<Leaderboard> GetLeaderboards(int raceId);
        List<Leaderboard> GetLeaderboardsForType(int raceId, string type);
        VehicleStatistics GetVehicle(int raceId, int vehicleId);
        List<VehicleStatistics> GetVehicles(int raceId, string team, string model, DateTime? manufacturingDate, MalfunctionStatus? status, decimal? distance);
        RaceStats GetRaceStats(int raceId);
    }
}
