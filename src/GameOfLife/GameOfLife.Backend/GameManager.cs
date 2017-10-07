using GameOfLife.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GameOfLife.Api.Model;

namespace GameOfLife.Backend
{
    public class GameManager : IGameManager, INotifyPropertyChanged
    {
        private ICollection<Coordinate> _clearTiles;

        public IList<Player> PlayerList { get; set; }

        public GameMap GameMap { get; private set; }

        public int GenerationPerRound { get; private set; }

        public int Round
        {
            get { return _round; }
            private set
            {
                _round = value;
                RaisePropertyChanged();
            }
        }

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
        private int _round;
        private int _generations;
        private bool _started;

        public event EventHandler<GameFinishedEventArgs> GameFinished;
        public event EventHandler<GenerationDoneEventArgs> GenerationDone;
        public event EventHandler<RoundDoneEventArgs> RoundDone;

        public GameManager()
        {
            PlayerList = new List<Player>();
            _clearTiles = new List<Coordinate>();
            _playerConfigs = new List<PlayerConfiguration>();
        }

        public void AddPlayer(PlayerConfiguration configuration)
        {
            _playerConfigs.Add(configuration);
        }

        public GameMap GenerateGameMap(GameConfiguration gameConfiguration)
        {
            GameMap = new GameMap { Tiles = new Tile[gameConfiguration.MapWidth][] };
            for (int i = 0; i < GameMap.Tiles.Length; i++)
            {
                GameMap.Tiles[i] = new Tile[gameConfiguration.MapHeight];
                for (int j = 0; j < GameMap.Tiles[i].Length; j++)
                {
                    GameMap.Tiles[i][j] = new Tile();
                }
            }
            GenerationPerRound = gameConfiguration.GenerationsPerRound;
            return GameMap;
        }


        public void SimulateRound(IEnumerable<PlayerAction> playerActions)
        {
            if (playerActions.Count() != PlayerList.Count)
            {
                throw new ArgumentException("The Amount of Player Actions didn't match the Amount of Players", nameof(playerActions));
            }
            for (int i = 0; i < GenerationPerRound; i++)
            {
                SimulateGeneration();
            }
            Round++;
            RaiseRoundDoneEvent(new RoundDoneEventArgs());
        }

        private void SimulateGeneration()
        {
            GameMap newGameMap = new GameMap();
            newGameMap.Tiles = new Tile[GameMap.Tiles.Length][];

            for (int j = 0; j < GameMap.Tiles.Length; j++)
            {
                newGameMap.Tiles[j] = new Tile[GameMap.Tiles[j].Length];
                for (int k = 0; k < GameMap.Tiles[j].Length; k++)
                {
                    var newTile = new Tile()
                    {
                        TileAttributes = GameMap.Tiles[j][k].TileAttributes
                    };
                    newGameMap.Tiles[j][k] = newTile;
                    var neighbours = getLivingNeighbours(j, k);

                    if (GameMap.Tiles[j][k].IsAlive)
                    {
                        HandleAliveTile(neighbours, GameMap.Tiles[j][k], newTile);
                    }
                    else
                    {
                        HandleDeadTile(neighbours, newGameMap, j, k);
                    }
                }
            }
            VisualizeGamestate(newGameMap);
            for (int j = 0; j < newGameMap.Tiles.Length; j++)
            {
                for (int k = 0; k < newGameMap.Tiles[j].Length; k++)
                {
                    GameMap.Tiles[j][k].Entity = newGameMap.Tiles[j][k].Entity;
                }
            }
            Generations++;
            RaiseGenerationDoneEvent(new GenerationDoneEventArgs());
        }

        private void HandleDeadTile(IEnumerable<Tile> neighbours, GameMap newGameMap, int j, int k)
        {
            var playerWantsHere = new List<bool>();
            var newAttribute = new Dictionary<EntityAttribute, int>();
            for (int i = 0; i < PlayerList.Count; i++)
            {
                int neighboursNeeded = 9;
                int presentNeighbours = 0;
                int dontNeedThatMany = 0;
                foreach (var neighbour in neighbours)
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
                    }
                }


                if (presentNeighbours >= neighboursNeeded && presentNeighbours <= dontNeedThatMany)
                {
                    playerWantsHere.Add(true);
                }
                else
                {
                    playerWantsHere.Add(false);
                }
            }
            if (playerWantsHere.Count(b => b) == 1)
            {
                newGameMap.Tiles[j][k].Entity = new Entity()
                {
                    EntityAttributes = newAttribute,
                    Owner = PlayerList[playerWantsHere.IndexOf(true)]
                };
            }
        }

        private void HandleAliveTile(IEnumerable<Tile> neighbours, Tile tile, Tile newTile)
        {
            var count = neighbours.Count(t => t.IsAlive && t.Entity.Owner == tile.Entity.Owner);
            if (CanSurvive(tile, count))
            {
                newTile.Entity = tile.Entity;
            }
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
                    GameMap.Tiles[coordinate.X][coordinate.Y].Entity = new Entity()
                    {
                        Owner = configuration.Player,
                        EntityAttributes = configuration.StartAttributes
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
            Debug.WriteLine("===============================================================");
            Debug.WriteLine("Visualizing Game State");
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                for (int j = 0; j < map.Tiles[i].Length; j++)
                {
                    var tile = map.Tiles[j][i];
                    if (tile.Entity == null)
                    {
                        Debug.Write("   ");
                    }
                    else
                    {
                        Debug.Write($" {tile.Entity.Owner.Name.Substring(0, 1)} ");
                    }
                }
                Debug.WriteLine("");
            }
        }

        protected virtual void RaiseGameFinishedEvent(GameFinishedEventArgs e)
        {
            GameFinished?.Invoke(this, e);
        }

        protected virtual void RaiseGenerationDoneEvent(GenerationDoneEventArgs e)
        {
            GenerationDone?.Invoke(this, e);
        }

        protected virtual void RaiseRoundDoneEvent(RoundDoneEventArgs e)
        {
            RoundDone?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
