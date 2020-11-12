using DakarRally.Interfaces.Repositories;
using DakarRally.Interfaces.Services;
using DakarRally.Models.Malfunctions;
using DakarRally.Models.Races;
using DakarRally.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DakarRally.BL
{
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _raceRepository;
        public RaceService(IRaceRepository raceRepository)
        {
            _raceRepository = raceRepository;
        }

        public Race CreateRace(int year)
        {
            return _raceRepository.CreateRace(year);
        }

        public Vehicle AddVehicle(int raceId, Vehicle model)
        {
            VehicleModel vehicle = MapVehicleToVehicleModel(model);
            VehicleModel newVehicle = _raceRepository.AddVehicle(raceId, vehicle);
            return MapVehicleModelTOVehicle(newVehicle);
        }

        public Vehicle UpdateVehicle(int raceId, int? id, Vehicle model)
        {
            VehicleModel vehicle = MapVehicleToVehicleModel(model);
            VehicleModel newVehicle = _raceRepository.UpdateVehicle(raceId, id.Value, vehicle);
            return MapVehicleModelTOVehicle(newVehicle);
        }

        public bool RemoveFromRace(int raceId, int id)
        {
            return _raceRepository.RemoveFromRace(raceId, id);
        }

        public bool StartRace(int raceId)
        {
            var startDate = DateTime.Now;
            Random rand = new Random();
            int raceDistance = 10000;

            var raceVehicles = _raceRepository.GetRaceVehicles(raceId);
            raceVehicles.ForEach(x => CalculateVehicleFinishStatistics(startDate, rand, raceDistance, x));
            var raceEndsAt = raceVehicles.Where(x => x.EndsRaceAt.HasValue)
                                         .Select(x => x.EndsRaceAt)
                                         .OrderByDescending(x => x)
                                         .FirstOrDefault();
            return _raceRepository.StartRaceAndUpdateVehicles(raceId, startDate, raceEndsAt.Value, raceVehicles);
        }

        public List<RaceVehicle> GetRaceVehicles(int raceId)
        {
            return _raceRepository.GetRaceVehicles(raceId);
        }

        public Race GetRace(int raceId)
        {
            return _raceRepository.GetRace(raceId);
        }

        #region PrivateMethods

        private static Vehicle MapVehicleModelTOVehicle(VehicleModel newVehicle)
        {
            if (newVehicle == null)
            {
                return null;
            }
            return new Vehicle()
            {
                Id = newVehicle.Id,
                ManufacturingDate = newVehicle.ManufacturingDate,
                Model = newVehicle.Model,
                TeamName = newVehicle.TeamName,
                VehicleType = newVehicle.VehicleTypeName,
                VehicleSubtype = newVehicle.VehicleSubtypeName,
            };
        }

        private static VehicleModel MapVehicleToVehicleModel(Vehicle model)
        {
            var vehicleType = VehicleType.GetVehicleType(model.VehicleType, model.VehicleSubtype);
            var vehicle = new VehicleModel()
            {
                ManufacturingDate = model.ManufacturingDate,
                Model = model.Model,
                TeamName = model.TeamName,
                VehicleTypeId = vehicleType.Type,
                VehicleSubtypeId = vehicleType.Subtype,
            };
            return vehicle;
        }

        private void CalculateVehicleFinishStatistics(DateTime startDate, Random rand, int raceDistance, RaceVehicle vehicle)
        {
            decimal finishTimeSeconds = ((decimal)raceDistance / (decimal)vehicle.VehicleTypeParameters.TopSpeed) * 3600m;
            var raceEndsAt = startDate.AddSeconds((double)finishTimeSeconds);
            int finishTime = (int)finishTimeSeconds;

            vehicle.Malfunctions = CreateVehicleMalfunctions(new List<MalfunctionStats>(), rand, startDate, ref raceEndsAt, startDate, startDate.AddHours(1), vehicle);
            vehicle.EndsRaceAt = raceEndsAt;
            decimal totalSecondsForRepairment = vehicle.Malfunctions.Where(x => x.EndAt.HasValue)
                                                            .Sum(x => (decimal)(x.EndAt.Value - x.StartAt).TotalSeconds);
            if (vehicle.Malfunctions.Any(x => x.Status == (short)MalfunctionStatus.Heavy))
            {
                vehicle.FinishTime = null;
                decimal raceTimeInSeconds = (decimal)(raceEndsAt - startDate).TotalSeconds - totalSecondsForRepairment;
                decimal distance = (((decimal)vehicle.VehicleTypeParameters.TopSpeed * 1000m) / 3600m) * raceTimeInSeconds;
                vehicle.DistanceInMeters = distance;
            }
            else
            {
                vehicle.FinishTime = (decimal)(raceEndsAt - startDate).TotalSeconds;
                vehicle.DistanceInMeters = raceDistance * 1000;
            }
        }

        /// <summary>
        /// Creates a list of all malfunctions that will happen in the race. Recursive function that exits if heavy malfunction happens or vehicle ends the race.
        /// </summary>
        /// <param name="malfunctions"></param>
        /// <param name="rand"></param>
        /// <param name="raceStartAt"></param>
        /// <param name="raceEndsAt"></param>
        /// <param name="startAt"></param>
        /// <param name="endAt"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        private List<MalfunctionStats> CreateVehicleMalfunctions(List<MalfunctionStats> malfunctions, Random rand, DateTime raceStartAt, ref DateTime raceEndsAt, DateTime startAt, DateTime endAt, RaceVehicle vehicle)
        {
            if (startAt >= raceEndsAt)
            {
                return malfunctions;
            }

            int hourInSeconds = 3600;
            if (endAt > raceEndsAt)
            {
                endAt = raceEndsAt;
                hourInSeconds = (int)(endAt - startAt).TotalSeconds;
            }

            var nextPeriodStart = endAt;
            if (rand.Next(100) < vehicle.VehicleTypeParameters.HeavyMalfunctionPercentage * 100)
            {
                var malfunction = CreateHeavyMalfunction(rand, startAt, hourInSeconds);
                malfunctions.Add(malfunction);
                raceEndsAt = malfunction.StartAt;
                return malfunctions;
            }
            else if (rand.Next(100) < vehicle.VehicleTypeParameters.LightMalfunctionPercentage * 100)
            {
                var malfunction = CreateLightMalfunction(rand, startAt, vehicle, hourInSeconds);
                malfunctions.Add(malfunction);
                nextPeriodStart = malfunction.EndAt.Value;
                raceEndsAt = raceEndsAt.AddSeconds(vehicle.VehicleTypeParameters.LightMalfunctionTime * 3600);
            }
            return CreateVehicleMalfunctions(malfunctions, rand, raceStartAt, ref raceEndsAt, nextPeriodStart, nextPeriodStart.AddHours(1), vehicle);
        }

        /// <summary>
        /// Creates an light malfunction for vehicle
        /// </summary>
        /// <param name="rand">Random object reference</param>
        /// <param name="startAt">Date from which malfunction can happen</param>
        /// <param name="vehicle">Vehicle for which malfunction is created</param>
        /// <param name="to">Number of seconds in which this malfunction can happen</param>
        /// <returns>New light malfunction</returns>
        private static MalfunctionStats CreateLightMalfunction(Random rand, DateTime startAt, RaceVehicle vehicle, int to)
        {
            MalfunctionStats malfunction = new MalfunctionStats();
            int lightMalfunctionAt = rand.Next(to);
            malfunction.Status = (short)MalfunctionStatus.Light;
            malfunction.StartAt = startAt.AddSeconds(lightMalfunctionAt);
            malfunction.EndAt = malfunction.StartAt.AddHours(vehicle.VehicleTypeParameters.LightMalfunctionTime);
            return malfunction;
        }

        /// <summary>
        /// Creates an heavy malfunction for vehicle 
        /// </summary>
        /// <param name="rand">Random object reference</param>
        /// <param name="startAt">Date from which malfunction can happen</param>
        /// <param name="to">Number of seconds in which this malfunction can happen</param>
        /// <returns>New heavy malfunction</returns>
        private static MalfunctionStats CreateHeavyMalfunction(Random rand, DateTime startAt, int to)
        {
            MalfunctionStats malfunction = new MalfunctionStats();
            int lightMalfunctionAt = rand.Next(to);
            malfunction.Status = (short)MalfunctionStatus.Heavy;
            malfunction.StartAt = startAt.AddSeconds(lightMalfunctionAt);
            malfunction.EndAt = null;
            return malfunction;
        }

        #endregion
    }
}