using DakarRally.Models.Races;
using DakarRally.Models.Vehicles;
using System.Collections.Generic;

namespace DakarRally.Interfaces.Services
{
    public interface IRaceService
    {
        Race CreateRace(int year);
        Vehicle AddVehicle(int raceId, Vehicle model);
        Vehicle UpdateVehicle(int raceId, int? id, Vehicle model);
        bool RemoveFromRace(int raceId, int id);
        bool StartRace(int raceId);
        List<RaceVehicle> GetRaceVehicles(int raceId);
        Race GetRace(int raceId);
    }
}
