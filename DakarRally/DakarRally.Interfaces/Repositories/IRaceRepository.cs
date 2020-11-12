using DakarRally.Models.Races;
using DakarRally.Models.Vehicles;
using System;
using System.Collections.Generic;

namespace DakarRally.Interfaces.Repositories
{
    public interface IRaceRepository
    {
        Race CreateRace(int year);
        VehicleModel AddVehicle(int raceId, VehicleModel vehicle);
        VehicleModel UpdateVehicle(int raceId, int value, VehicleModel vehicle);
        bool RemoveFromRace(int raceId, int id);
        bool StartRaceAndUpdateVehicles(int raceId, DateTime startDate, DateTime raceEndsAt, List<RaceVehicle> raceVehicles);
        List<RaceVehicle> GetRaceVehicles(int raceId);
        Race GetRace(int raceId);
    }
}
