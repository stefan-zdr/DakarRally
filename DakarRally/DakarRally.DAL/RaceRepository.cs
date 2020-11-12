using DakarRally.Interfaces.Repositories;
using DakarRally.Models.Races;
using DakarRally.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Data.Entity;

namespace DakarRally.DAL
{
    public class RaceRepository : BaseRepository, IRaceRepository
    {
        public Race CreateRace(int year)
        {
            using (var context = GetContext())
            {
                var newRace = context.Races
                    .Add(new Model.Race()
                    {
                        Year = year,
                        Status = (short)RaceStatus.Pending,
                    });
                context.SaveChanges();

                return new Race()
                {
                    Id = newRace.Id,
                    Year = newRace.Year,
                    Status = (RaceStatus)newRace.Status,
                };
            }
        }

        // Here would be good to use TransactionScope but Sql Server CE doesn't support it
        public VehicleModel AddVehicle(int raceId, VehicleModel vehicle)
        {
            using (var context = GetContext())
            {
                if (context.Races.Any(x => x.Id == raceId && x.Status == (short)RaceStatus.Pending))
                {
                    var newVehicle = context.Vehicles.Add(new Model.Vehicle()
                    {
                        TeamName = vehicle.TeamName,
                        Model = vehicle.Model,
                        ManufacturingDate = vehicle.ManufacturingDate,
                        VehicleTypeId = vehicle.VehicleTypeId,
                        VehicleSubtypeId = vehicle.VehicleSubtypeId,
                    });
                    context.SaveChanges();
                    context.RaceVehicles.Add(new Model.RaceVehicle()
                    {
                        RaceId = raceId,
                        VehicleId = newVehicle.Id,
                    });
                    context.SaveChanges();
                    return new VehicleModel()
                    {
                        Id = newVehicle.Id,
                        Model = newVehicle.Model,
                        TeamName = newVehicle.TeamName,
                        ManufacturingDate = newVehicle.ManufacturingDate,
                        VehicleTypeId = newVehicle.VehicleTypeId,
                        VehicleSubtypeId = newVehicle.VehicleSubtypeId,
                    };
                }
                return null;
            }
        }

        public VehicleModel UpdateVehicle(int raceId, int id, VehicleModel vehicle)
        {
            using (var context = GetContext())
            {
                if (context.RaceVehicles.Any(x => x.RaceId == raceId && x.VehicleId == id && x.Race.Status == (short)RaceStatus.Pending))
                {
                    var dbVehicle = context.Vehicles.SingleOrDefault(x => x.Id == id);
                    dbVehicle.ManufacturingDate = vehicle.ManufacturingDate;
                    dbVehicle.Model = vehicle.Model;
                    dbVehicle.TeamName = vehicle.TeamName;
                    dbVehicle.VehicleTypeId = vehicle.VehicleTypeId;
                    dbVehicle.VehicleSubtypeId = vehicle.VehicleSubtypeId;
                    context.SaveChanges();
                    return new VehicleModel()
                    {
                        Id = dbVehicle.Id,
                        Model = dbVehicle.Model,
                        TeamName = dbVehicle.TeamName,
                        ManufacturingDate = dbVehicle.ManufacturingDate,
                        VehicleTypeId = dbVehicle.VehicleTypeId,
                        VehicleSubtypeId = dbVehicle.VehicleSubtypeId,
                    };
                }
                return null;
            }
        }

        public bool RemoveFromRace(int raceId, int id)
        {
            using (var context = GetContext())
            {
                var dbRaceVehicle = context.RaceVehicles.SingleOrDefault(x => x.RaceId == raceId && x.VehicleId == id && x.Race.Status == (short)RaceStatus.Pending);

                if (dbRaceVehicle == null)
                {
                    return false;
                }

                context.RaceVehicles.Remove(dbRaceVehicle);
                context.SaveChanges();
                return true;
            }
        }

        public bool StartRaceAndUpdateVehicles(int raceId, DateTime startDate, DateTime raceEndsAt, List<RaceVehicle> raceVehicles)
        {
            using (var context = GetContext())
            {
                var dbRace = context.Races.SingleOrDefault(x => x.Id == raceId);
                if (context.Races.Any(x => x.Status == (short)RaceStatus.Running && x.EndsAt >= DateTime.Now)
                    || dbRace == null
                    || dbRace.Year != startDate.Year)
                {
                    return false;
                }

                UpdateRace(raceId, startDate, raceEndsAt, context);
                UpdateRaceVehicles(raceId, raceVehicles, context);

                context.SaveChanges();
                return true;
            }
        }

        public List<RaceVehicle> GetRaceVehicles(int raceId)
        {
            using (var context = GetContext())
            {
                return context.RaceVehicles
                    .Include(x => x.Vehicle)
                    .Include(x => x.Vehicle.VehicleType)
                    .Include(x => x.Vehicle.VehicleSubtype)
                    .Where(x => x.RaceId == raceId)
                    .Select(x => new
                    {
                        x.VehicleId,
                        x.Vehicle.TeamName,
                        x.Vehicle.Model,
                        x.Vehicle.ManufacturingDate,
                        VehicleTypeName = x.Vehicle.VehicleType.Name,
                        x.Vehicle.VehicleSubtype,
                        x.EndsRaceAt,
                        Malfunctions = context.Malfunctions.Where(m => m.RaceId == raceId && m.VehicleId == x.VehicleId)
                                                           .AsEnumerable(),
                        x.FinishTime,
                        x.Distance,
                        VehicleType = x.Vehicle.VehicleSubtype ?? x.Vehicle.VehicleType,
                    })
                    .AsEnumerable()
                    .Select(x => new RaceVehicle()
                    {
                        VehicleId = x.VehicleId,
                        TeamName = x.TeamName,
                        Model = x.Model,
                        ManufacturingDate = x.ManufacturingDate,
                        Type = x.VehicleTypeName,
                        Subtype = x.VehicleSubtype?.Name,
                        EndsRaceAt = x.EndsRaceAt,
                        Malfunctions = x.Malfunctions.Select(m => new Models.Malfunctions.MalfunctionStats()
                        {
                            Status = m.Status,
                            StartAt = m.StartAt,
                            EndAt = m.EndAt,
                        }).ToList(),
                        FinishTime = x.FinishTime,
                        DistanceInMeters = x.Distance,
                        VehicleTypeParameters = GetVehicleTypeParameters(x.VehicleType),
                    })
                    .ToList();
            }
        }

        public Race GetRace(int raceId)
        {
            using (var context = GetContext())
            {
                return context.Races
                    .Where(x => x.Id == raceId)
                    .Select(x => new Race()
                    {
                        Id = x.Id,
                        StartsAt = x.StartsAt,
                        EndssAt = x.EndsAt,
                        Year = x.Year,
                        Status = (RaceStatus)x.Status,
                    }).SingleOrDefault();
            }
        }

        #region Private Methods

        private static void UpdateRace(int raceId, DateTime startDate, DateTime raceEndsAt, Model.DakarRallyEntities context)
        {
            var race = context.Races.SingleOrDefault(x => x.Id == raceId);
            race.StartsAt = startDate;
            race.EndsAt = raceEndsAt;
            race.Status = (short)RaceStatus.Running;
        }

        private static void UpdateRaceVehicles(int raceId, List<RaceVehicle> raceVehicles, Model.DakarRallyEntities context)
        {
            var dbRaceVehicles = context.RaceVehicles.Where(x => x.RaceId == raceId);
            foreach (var vehicle in dbRaceVehicles)
            {
                var newVehicle = raceVehicles.SingleOrDefault(x => x.VehicleId == vehicle.VehicleId);
                int malfunctionId = 1;
                context.Malfunctions.AddRange(newVehicle.Malfunctions.Select(x => new Model.Malfunction()
                {
                    RaceId = raceId,
                    VehicleId = vehicle.VehicleId,
                    MalfunctionId = malfunctionId++,
                    Status = x.Status,
                    StartAt = x.StartAt,
                    EndAt = x.EndAt,
                }));
                vehicle.FinishTime = newVehicle.FinishTime;
                vehicle.Distance = newVehicle.DistanceInMeters;
            }
        }

        private VehicleTypeParameters GetVehicleTypeParameters(Model.VehicleType vehicleType)
        {
            return new VehicleTypeParameters()
            {
                VehicleTypeId = vehicleType.Id,
                TopSpeed = vehicleType.TopSpeedKmH.GetValueOrDefault(),
                LightMalfunctionTime = vehicleType.LightMalfunctionHours.GetValueOrDefault(),
                LightMalfunctionPercentage = vehicleType.PercentageForLightMalfunction.GetValueOrDefault(),
                HeavyMalfunctionPercentage = vehicleType.PercentageForHeavyMalfanction.GetValueOrDefault(),
            };
        }

        #endregion
    }
}