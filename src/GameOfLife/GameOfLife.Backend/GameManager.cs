using GameOfLife.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using GameOfLife.Api.Model;

namespace GameOfLife.Backend
{
    public class GameManager : IGameManager, INotifyPropertyChanged
    {
        private ICollection<Coordinate> _clearTiles;

        private ICollection<HotSpot> _hotSpots;

        public IList<Player> PlayerList { get; set; }

        public GameMap GameMap { get; private set; }

        public int GenerationPerRound { get; private set; }

        public int Generations
        {
            get { return _generations; }
            private set
            {
                _generations = value;
                RaisePropertyChanged();
            }
        }

        public bool Started
        {
            get { return _started; }
            private set
            {
                _started = value;
                RaisePropertyChanged();
            }
        }

        private ICollection<PlayerConfiguration> _playerConfigs;
        private int _generations;
        private bool _started;
        private double _gameMapDiagonal;
        private Random _random = new Random();

        public event EventHandler<GameFinishedEventArgs> GameFinished;
        public event EventHandler<GenerationDoneEventArgs> GenerationDone;

        public GameManager()
        {
            PlayerList = new List<Player>();
            _clearTiles = new List<Coordinate>();
            _playerConfigs = new List<PlayerConfiguration>();
            _started = false;
        }

        public void AddPlayer(PlayerConfiguration configuration)
        {
            _playerConfigs.Add(configuration);
        }

        public GameMap GenerateGameMap(GameConfiguration gameConfiguration)
        {
            _random = new Random(gameConfiguration.Seed);
            _gameMapDiagonal = CalculatePytagoras(gameConfiguration.MapWidth, gameConfiguration.MapHeight);
            _hotSpots = GenerateHotSpots(gameConfiguration.MapWidth, gameConfiguration.MapHeight);
            GameMap = new GameMap { Tiles = new Tile[gameConfiguration.MapWidth][] };
            for (int i = 0; i < GameMap.Tiles.Length; i++)
            {
                GameMap.Tiles[i] = new Tile[gameConfiguration.MapHeight];
                for (int j = 0; j < GameMap.Tiles[i].Length; j++)
                {
                    GameMap.Tiles[i][j] = new Tile(){X=i,Y=j};
                    GameMap.Tiles[i][j].Temperature.Value = CalculateTemperature(i, j, _hotSpots);
                }
            }
            foreach (var hotSpot in _hotSpots)
            {
                GameMap.Tiles[hotSpot.X][hotSpot.Y].DebugHighlight = true;
            }

            GenerationPerRound = gameConfiguration.GenerationsPerRound;
            return GameMap;
        }

        public void SimulateGeneration(IEnumerable<PlayerAction> playerActions)
        {
            try
            {

                var newHotSpots = new List<HotSpot>();
                foreach (var playerAction in playerActions)
                {
                    foreach (var temp in playerAction.TemperatureManipulations)
                    {
                        var hotSpot = new HotSpot
                        {
                            X = temp.Tile.X,
                            Y = temp.Tile.Y,
                            Temperature = (int) (10.0 * Math.Max(temp.Change / 10.0, 1.0)) +
                                          Temperature.MedianTemperature,
                            FalloffStep = 1.5d
                        };
                        newHotSpots.Add(hotSpot);
                    }
                }


            GameMap newGameMap = new GameMap();
            newGameMap.Tiles = new Tile[GameMap.Tiles.Length][];

            for (int j = 0; j < GameMap.Tiles.Length; j++)
            {
                newGameMap.Tiles[j] = new Tile[GameMap.Tiles[j].Length];
                for (int k = 0; k < GameMap.Tiles[j].Length; k++)
                {
                    GameMap.Tiles[j][k].Temperature.Value = CalculateTemperature(j, k, newHotSpots);
                        var newTile = new Tile()
                    {
                        // hier war was mit Tileattributes
                    };

                    newGameMap.Tiles[j][k] = newTile;
                    var neighbours = getLivingNeighbours(j, k);

                    HandleTile(neighbours, newGameMap, j, k);
                }
            }
            VisualizeGamestate(newGameMap);
            for (int j = 0; j < newGameMap.Tiles.Length; j++)
            {
                for (int k = 0; k < newGameMap.Tiles[j].Length; k++)
                {
                    var tile = newGameMap.Tiles[j][k];
                    if (tile.IsAlive)
                    {
                        var topTile = newGameMap.Tiles[j][WrapIntToBounds(k - 1, newGameMap.Tiles[j].Length)];
                        var southTile = newGameMap.Tiles[j][WrapIntToBounds(k + 1, newGameMap.Tiles[j].Length)];
                        var eastTile = newGameMap.Tiles[WrapIntToBounds(j + 1, newGameMap.Tiles.Length)][k];
                        var westTile = newGameMap.Tiles[WrapIntToBounds(j - 1, newGameMap.Tiles.Length)][k];

                        var entity = newGameMap.Tiles[j][k].Entity;
                        entity.NeighborInfo.HasNorthNeighbor = topTile.IsAlive && topTile.Entity.Owner == entity.Owner;
                        entity.NeighborInfo.HasSouthNeighbor = southTile.IsAlive && southTile.Entity.Owner == entity.Owner;
                        entity.NeighborInfo.HasEastNeighbor = eastTile.IsAlive && eastTile.Entity.Owner == entity.Owner;
                        entity.NeighborInfo.HasWestNeighbor = westTile.IsAlive && westTile.Entity.Owner == entity.Owner;
                    }

                    GameMap.Tiles[j][k].Entity = tile.Entity;
                }
            }
            Generations++;
            RaiseGenerationDoneEvent(new GenerationDoneEventArgs());
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public class PlayerDemand
        {
            public double CombatSkill { get; set; }
            public Player Player { get; set; }
            public Dictionary<EntityAttribute, int> NewAttributes { get; set; }
            public Entity AttackingEntity { get; set; }
        }

        private void HandleTile(IEnumerable<Tile> neighbours, GameMap newGameMap, int j, int k)
        {
            var currentTile = GameMap.Tiles[j][k];

            var playerWantsHere = new List<PlayerDemand>();
            var enumerable = neighbours as Tile[] ?? neighbours.ToArray();
            for (int i = 0; i < PlayerList.Count; i++)
            {
                var newAttribute = new Dictionary<EntityAttribute, int>();
                int neighboursNeeded = 9;
                int presentNeighbours = 0;
                int dontNeedThatMany = 0;
                double maxCombatSkill = 0;
                Entity attackingEntity = null;

                foreach (var neighbour in enumerable)
                {
                    if (neighbour.IsAlive && neighbour.Entity.Owner == PlayerList[i])
                    {
                        neighboursNeeded = Math.Min(neighboursNeeded,
                            neighbour.Entity.EntityAttributes[EntityAttribute.MinNeighboursForDead]);
                        dontNeedThatMany = Math.Max(neighboursNeeded,
                            neighbour.Entity.EntityAttributes[EntityAttribute.MaxNeighboursForDead]);
                        presentNeighbours++;
                        foreach (var entityAttribute in neighbour.Entity.EntityAttributes)
                        {
                            newAttribute[entityAttribute.Key] = entityAttribute.Value;
                        }
                        attackingEntity = neighbour.Entity;
                        maxCombatSkill = Math.Max(maxCombatSkill, CalculateHeatResistance(neighbour.Entity, GameMap.Tiles[j][k]));
                    }
                }


                if (presentNeighbours >= neighboursNeeded && presentNeighbours <= dontNeedThatMany)
                {
                    playerWantsHere.Add(new PlayerDemand()
                    {
                        Player = PlayerList[i],
                        NewAttributes = newAttribute,
                        CombatSkill = maxCombatSkill,
                        AttackingEntity = attackingEntity
                    });
                }
            }
            PlayerDemand successfulDemand = null;
            if (playerWantsHere.Count > 0)
            {
                foreach (var playerDemand in playerWantsHere)
                {
                    if (successfulDemand == null || successfulDemand.CombatSkill < playerDemand.CombatSkill)
                        successfulDemand = playerDemand;
                }
            }


            if(currentTile.IsAlive && CanSurvive(currentTile, enumerable.Count(t => t.IsAlive && t.Entity.Owner == currentTile.Entity.Owner)))
            {
                var defendingSkill = CalculateHeatResistance(currentTile.Entity, currentTile);
                if (successfulDemand == null || successfulDemand.CombatSkill < defendingSkill || successfulDemand.Player == currentTile.Entity.Owner)
                {
                    newGameMap.Tiles[j][k].Entity = currentTile.Entity;
                    return;
                }
            }
            if (successfulDemand == null) return;

            var entity = new Entity()
            {
                EntityAttributes = successfulDemand.NewAttributes,
                Owner = successfulDemand.Player,
                Efficiency = successfulDemand.AttackingEntity.Efficiency,
                IdealTemperature = successfulDemand.AttackingEntity.IdealTemperature,
                Resitance = successfulDemand.AttackingEntity.Resitance
            };
            newGameMap.Tiles[j][k].Entity = entity;
            entity.Owner.Score++;

        }

        private static bool CanSurvive(Tile tile, int numberAlliedNeighbour)
        {
            return tile.Entity.EntityAttributes[EntityAttribute.MinNeighboursForLife] <=
                   numberAlliedNeighbour && tile.Entity
                       .EntityAttributes[EntityAttribute.MaxNeighboursForLife] >= numberAlliedNeighbour;
        }

        public void Start()
        {
            foreach (var configuration in _playerConfigs)
            {
                PlayerList.Add(configuration.Player);
                foreach (var coordinate in configuration.Coordinates)
                {
                    if (GameMap.Tiles[coordinate.X][coordinate.Y].IsAlive)
                    {
                        _clearTiles.Add(coordinate);
                    }
                    var temperature = PlayerList.Count == 1 ? 30 : 0;
                    var efficiency = 10;
                    var resistance = 0.7d;
                    GameMap.Tiles[coordinate.X][coordinate.Y].Entity = new Entity()
                    {
                        Owner = configuration.Player,
                        EntityAttributes = configuration.StartAttributes,
                        IdealTemperature = temperature,
                        Efficiency = efficiency,
                        Resitance = resistance
                    };
                }
            }

            foreach (var coordinate in _clearTiles)
            {
                GameMap.Tiles[coordinate.X][coordinate.Y].Entity = null;
            }
            Started = true;
        }

        private IEnumerable<Tile> getLivingNeighbours(int x, int y)
        {
            List<Tile> returnValue = new List<Tile>();

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0) continue;
                    int xCoord = WrapIntToBounds(x + i, GameMap.Tiles.Length);
                    int yCoord = WrapIntToBounds(y + j, GameMap.Tiles[0].Length);
                    returnValue.Add(GameMap.Tiles[xCoord][yCoord]);
                }
            }
            return returnValue;
        }

        private int WrapIntToBounds(int i, int max)
        {
            return i < 0 ? max + i : i >= max ? i - max : i;
        }

        public void VisualizeGamestate(GameMap map)
        {
            //Debug.WriteLine("===============================================================");
            //Debug.WriteLine("Visualizing Game State");
            //for (int i = 0; i < map.Tiles.Length; i++)
            //{
            //    for (int j = 0; j < map.Tiles[i].Length; j++)
            //    {
            //        var tile = map.Tiles[j][i];
            //        if (tile.Entity == null)
            //        {
            //            Debug.Write("   ");
            //        }
            //        else
            //        {
            //            Debug.Write($" {tile.Entity.Owner.Name.Substring(0, 1)} ");
            //        }
            //    }
            //    Debug.WriteLine("");
            //}
        }

        private ICollection<HotSpot> GenerateHotSpots(int width, int height)
        {
            var result = new List<HotSpot>();
            int tileCount = width * height;
            int hotSpotCount = 30;//_random.Next(tileCount / 1000, tileCount / 200);
            for (int i = 0; i < hotSpotCount; i++)
            {
                result.Add(new HotSpot()
                {
                    X = _random.Next(0, width),
                    Y = _random.Next(0, height),
                    Temperature = _random.NextDouble() > 0.5d ? Temperature.MinTemperature : Temperature.MaxTemperature,
                    FalloffStep = 1.5d
                });
            }
            return result;
        }

        private double CalculateTemperature(int x, int y, IEnumerable<HotSpot> hotSpots)
        {
            return hotSpots.Select(h => CalculateTemperatureForHotSpot(x, y, h)).Select(v => v - Temperature.MedianTemperature).Where(v => Math.Abs(v) > 0.1).ToList().Sum() + Temperature.MedianTemperature;
        }

        private double CalculateTemperatureForHotSpot(int x, int y, HotSpot spot)
        {
            if (spot.Temperature > 0)
            {
                return Clamp(spot.Temperature - spot.FalloffStep *
                             CalculatePytagoras(SpecialDistanceClampToNumberSpaceAwesomeness(spot.X - x, GameMap.Tiles.Length / 2), SpecialDistanceClampToNumberSpaceAwesomeness(spot.Y - y, GameMap.Tiles[0].Length / 2)),
                    Temperature.MedianTemperature, Temperature.MaxTemperature);

            }
            else
            {
                return Clamp(spot.Temperature + spot.FalloffStep *
                             CalculatePytagoras(SpecialDistanceClampToNumberSpaceAwesomeness(spot.X - x, GameMap.Tiles.Length / 2), SpecialDistanceClampToNumberSpaceAwesomeness(spot.Y - y, GameMap.Tiles[0].Length / 2)),
                    Temperature.MinTemperature, Temperature.MedianTemperature);
            }
        }

        private double SpecialDistanceClampToNumberSpaceAwesomeness(double value, double max)
        {
            value = Math.Abs(value);
            if (value < max) return value;

            return max - (value - max);
        }

        private double Clamp(double value, double min, double max)
        {
            return value > min ? value < max ? value : max : min;
        }

        private double CalculatePytagoras(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        private class HotSpot
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Temperature { get; set; }
            public double FalloffStep { get; set; }
        }

        private double CalculateHeatResistance(Entity entity, Tile target)
        {
            if (target.Temperature.Value <= entity.IdealTemperature)
                return Clamp(
                    entity.Resitance * (target.Temperature.Value - entity.IdealTemperature) +
                    entity.Efficiency, 0, entity.Efficiency);
            return Clamp(-entity.Resitance * (target.Temperature.Value - entity.IdealTemperature) +
                         entity.Efficiency, 0, entity.Efficiency);
        }

        protected virtual void RaiseGameFinishedEvent(GameFinishedEventArgs e)
        {
            GameFinished?.Invoke(this, e);
        }

        protected virtual void RaiseGenerationDoneEvent(GenerationDoneEventArgs e)
        {
            GenerationDone?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
