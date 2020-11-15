using DakarRally.Interfaces.Services;
using DakarRally.Models;
using DakarRally.Models.Leaderboards;
using DakarRally.Models.Malfunctions;
using DakarRally.Models.Races;
using DakarRally.Models.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DakarRally.BAL.Services
{
    public class LeaderboardsService : ILeaderboardsService
    {
        private readonly IRaceService _raceService;
        public LeaderboardsService(IRaceService raceService)
        {
            _raceService = raceService;
        }

        public List<Leaderboard> GetLeaderboards(int raceId)
        {
            var leaderboard = GetLeaderboard(raceId, DateTime.Now);
            if (leaderboard == null)
            {
                return null;
            }

            return leaderboard.Select(x => new Leaderboard()
            {
                Position = x.Position,
                TeamName = x.TeamName,
                Model = x.Model,
                Type = x.Type,
                Subtype = x.Subtype,
                MalfunctionStatus = x.MalfunctionStatus,
            }).ToList();
        }

        public List<Leaderboard> GetLeaderboardsForType(int raceId, string type)
        {
            var leaderboard = GetLeaderboard(raceId, DateTime.Now);
            if (leaderboard == null || !VehicleType.ValidateType(type))
            {
                return null;
            }

            return leaderboard.Where(x => x.Type == type).Select(x => new Leaderboard()
            {
                Position = x.Position,
                TeamName = x.TeamName,
                Model = x.Model,
                Type = x.Type,
                Subtype = x.Subtype,
                MalfunctionStatus = x.MalfunctionStatus,
            }).ToList();
        }

        public RaceStats GetRaceStats(int raceId)
        {
            var leaderboardDate = DateTime.Now;
            var leaderboard = GetLeaderboard(raceId, leaderboardDate);
            if (leaderboard == null)
            {
                return null;
            }

            return new RaceStats()
            {
                Status = GetRaceStatus(leaderboard, raceId, leaderboardDate),
                VehiclesNumberByStatus = leaderboard.GroupBy(x => x.MalfunctionStatus).ToDictionary(x => x.Key, x => x.Count()),
                VehiclesNumberByType = leaderboard.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.Count()),
            };
        }

        public VehicleStatistics GetVehicle(int raceId, int vehicleId)
        {
            var leaderboard = GetLeaderboard(raceId, DateTime.Now);
            if (leaderboard == null)
            {
                return null;
            }
            var vehicle = leaderboard.SingleOrDefault(x => x.VehicleId == vehicleId);
            if (vehicle == null)
            {
                return null;
            }
            return new VehicleStatistics()
            {
                Distance = vehicle.Distance,
                Status = vehicle.MalfunctionStatus,
                FinishTime = vehicle.FinishTime,
                NumberOfMalfunctions = vehicle.Malfunctions.Count(),
                CurrentMalfunctionStartAt = GetCurrentMalfunction(vehicle, DateTime.Now)?.StartAt,
            };
        }

        public List<VehicleStats> GetVehicles(int raceId, string team, string model, DateTime? manufacturingDate, MalfunctionStatus? status, decimal? distance, string sortOrder)
        {
            var leaderboard = GetLeaderboard(raceId, DateTime.Now);
            if (leaderboard == null)
            {
                return null;
            }

            var vehicles = leaderboard.Where(x => (string.IsNullOrEmpty(team) || x.TeamName == team)
                                          && (string.IsNullOrEmpty(model) || x.Model == model)
                                          && (!manufacturingDate.HasValue || x.ManufacturingDate == manufacturingDate)
                                          && (!status.HasValue || x.MalfunctionStatus == status)
                                          && (!distance.HasValue || x.Distance == distance));

            return SortVehiclesBy(vehicles, sortOrder).Select(x => new VehicleStats()
            {
                Id = x.VehicleId,
                TeamName = x.TeamName,
                Model = x.Model,
                ManufacturingDate = x.ManufacturingDate,
                Distance = x.Distance,
                Status = x.MalfunctionStatus,
                VehicleType = x.Type,
                VehicleSubtype = x.Subtype
            }).ToList();
        }

        #region Private Methods

        private IEnumerable<LeaderboardVehicle> SortVehiclesBy(IEnumerable<LeaderboardVehicle> vehicles, string sortOrder)
        {
            switch (sortOrder)
            {
                case "team":
                    return vehicles.OrderBy(x => x.TeamName);
                case "team_desc":
                    return vehicles.OrderByDescending(x => x.TeamName);
                case "model":
                    return vehicles.OrderBy(x => x.Model);
                case "model_desc":
                    return vehicles.OrderByDescending(x => x.Model);
                case "manufacturingDate":
                    return vehicles.OrderBy(x => x.ManufacturingDate);
                case "manufacturingDate_desc":
                    return vehicles.OrderByDescending(x => x.ManufacturingDate);
                case "status":
                    return vehicles.OrderBy(x => x.MalfunctionStatus);
                case "status_desc":
                    return vehicles.OrderByDescending(x => x.MalfunctionStatus);
                case "distance":
                    return vehicles.OrderBy(x => x.Distance);
                case "distance_desc":
                    return vehicles.OrderByDescending(x => x.Distance);
                default:
                    return vehicles;
            }
        }

        private MalfunctionStats GetCurrentMalfunction(LeaderboardVehicle vehicle, DateTime currentDate)
        {
            return vehicle.Malfunctions.FirstOrDefault(x => x.StartAt <= currentDate && (x.EndAt == null || x.EndAt.Value >= currentDate));
        }

        private IEnumerable<LeaderboardVehicle> GetLeaderboard(int raceId, DateTime leaderboardDate)
        {
            var vehicles = _raceService.GetRaceVehicles(raceId);
            Race race = _raceService.GetRace(raceId);
            IEnumerable<LeaderboardVehicle> leaderboardVehicles;
            if (race.Status == RaceStatus.Running && race.EndssAt.HasValue && race.EndssAt > leaderboardDate && race.StartsAt.HasValue && vehicles != null)
            {
                leaderboardVehicles = vehicles
                    .Select(x => new LeaderboardVehicle()
                    {
                        Distance = ((decimal)x.VehicleTypeParameters.TopSpeed * 1000m / 3600m) * GetVehiclesRaceTimeInSeconds(x, leaderboardDate, race.StartsAt.Value),
                        VehicleId = x.VehicleId,
                        Model = x.Model,
                        TeamName = x.TeamName,
                        ManufacturingDate = x.ManufacturingDate,
                        Type = x.Type,
                        Subtype = x.Subtype,
                        TopSpeed = x.VehicleTypeParameters.TopSpeed,
                        Malfunctions = x.Malfunctions,
                        RaceStartAt = race.StartsAt,
                        LeaderboardForDateTime = leaderboardDate,
                        MalfunctionStatus = x.MalfunctionStatus,
                    })
                    .OrderByDescending(x => x.Distance);
            }
            else
            {
                leaderboardVehicles = vehicles
                    .Select(x => new LeaderboardVehicle()
                    {
                        VehicleId = x.VehicleId,
                        Model = x.Model,
                        TeamName = x.TeamName,
                        ManufacturingDate = x.ManufacturingDate,
                        Type = x.Type,
                        Subtype = x.Subtype,
                        FinishTime = x.FinishTime,
                        TopSpeed = x.VehicleTypeParameters.TopSpeed,
                        Malfunctions = x.Malfunctions,
                        RaceStartAt = race.StartsAt,
                        LeaderboardForDateTime = leaderboardDate,
                        MalfunctionStatus = x.MalfunctionStatus,
                        Distance = x.DistanceInMeters,
                    })
                    .OrderByDescending(x => x.Distance)
                    .ThenBy(x => x.FinishTime);
            }

            int position = 1;
            return leaderboardVehicles
                .Select(x => new LeaderboardVehicle()
                {
                    VehicleId = x.VehicleId,
                    Position = position++,
                    Model = x.Model,
                    TeamName = x.TeamName,
                    ManufacturingDate = x.ManufacturingDate,
                    Type = x.Type,
                    Subtype = x.Subtype,
                    FinishTime = x.FinishTime,
                    TopSpeed = x.TopSpeed,
                    Malfunctions = x.Malfunctions,
                    RaceStartAt = race.StartsAt,
                    LeaderboardForDateTime = leaderboardDate,
                    MalfunctionStatus = x.MalfunctionStatus,
                    Distance = x.Distance,
                });
        }

        private decimal GetVehiclesRaceTimeInSeconds(RaceVehicle raceVehicle, DateTime leaderboardDate, DateTime raceStartAt)
        {
            var malfunction = raceVehicle.Malfunctions.FirstOrDefault(x => x.StartAt <= leaderboardDate
                                                               && (x.EndAt == null || x.EndAt >= leaderboardDate));

            double totalSecondsForRepairment = raceVehicle.Malfunctions.Where(x => x.StartAt <= leaderboardDate
                                                         && x.EndAt.HasValue
                                                         && x.EndAt <= leaderboardDate)
                                                            .Sum(x => (x.EndAt.Value - x.StartAt).TotalSeconds);
            raceVehicle.MalfunctionStatus = MalfunctionStatus.None;
            if (malfunction != null)
            {
                totalSecondsForRepairment += (leaderboardDate - malfunction.StartAt).TotalSeconds;
                raceVehicle.MalfunctionStatus = (MalfunctionStatus)malfunction.Status;
            }
            return (decimal)(leaderboardDate - raceStartAt).TotalSeconds - (decimal)totalSecondsForRepairment;
        }

        private RaceStatus GetRaceStatus(IEnumerable<LeaderboardVehicle> leaderboard, int raceId, DateTime leaderboardDate)
        {
            Race race = _raceService.GetRace(raceId);
            if (race.Status == RaceStatus.Running && race.EndssAt <= leaderboardDate)
            {
                return RaceStatus.Finished;
            }
            return race.Status;
        }

        #endregion
    }
}
