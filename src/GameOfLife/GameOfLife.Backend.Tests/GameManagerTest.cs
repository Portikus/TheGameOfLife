using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfLife.Api.Model;
using NUnit.Framework;

namespace GameOfLife.Backend.Tests
{
    public class GameManagerTest
    {
        private GameManager _systemUnderTest;
        private int _playerNumber = 5;


        [SetUp]
        public void SetUp()
        {
            _systemUnderTest = new GameManager();
        }

        [Test]
        public void TestGenerateGameMap()
        {
            const int size = 10;
            var gameMap = GenerateGameMap(size);

            Assert.That(gameMap.Tiles, Has.Length.EqualTo(size));
            Assert.That(gameMap.Tiles[0], Has.Length.EqualTo(size));
            Assert.That(gameMap.Tiles[2][5].Entity, Is.Null);
        }

        [Test]
        public void TestAddPlayer()
        {
            var gameMap = CreateValidGameState();
        }

        [Test]
        public void TestConflictingPlayer()
        {
            var gameMap = GenerateGameMap(10);

            for (var i = 0; i < 3; i++)
            {
                var coordinates = new List<Coordinate>
                {
                    new Coordinate()
                    {
                        X = 3,
                        Y = 4
                    }
                };
                var startAttributes = GenerateStartAttributes();

                _systemUnderTest.AddPlayer(new PlayerConfiguration()
                {
                    Coordinates = coordinates,
                    StartAttributes = startAttributes,
                    Player = new Player()
                });
            }

            _systemUnderTest.Start();

            Assert.That(gameMap.Tiles[3][4], Has.Property(nameof(Tile.Entity)).Null);
        }

        private static Dictionary<EntityAttribute, int> GenerateStartAttributes()
        {
            var startAttributes = new Dictionary<EntityAttribute, int>();
            startAttributes[EntityAttribute.MaxNeighboursForDead] = 6;
            startAttributes[EntityAttribute.MaxNeighboursForLife] = 7;
            startAttributes[EntityAttribute.MinNeighboursForDead] = 1;
            startAttributes[EntityAttribute.MinNeighboursForLife] = 0;
            return startAttributes;
        }

        [Test]
        public void TestMissingPlayerAction()
        {
            CreateValidGameState();
            Assert.That(() => _systemUnderTest.SimulateRound(new List<PlayerAction>()), Throws.Exception);
        }

        [Test]
        public void TestSimulateRound()
        {
            var gameMap = CreateValidGameState();
            var oldTiles = gameMap.Tiles.Clone();
            var actions = new List<PlayerAction>();
            foreach (var player in _systemUnderTest.PlayerList)
            {
                actions.Add(new PlayerAction()
                {
                    Player = player
                });
            }
            _systemUnderTest.SimulateRound(actions);
            Assert.That(gameMap.Tiles, Is.Not.EqualTo(oldTiles));


        }

        private GameMap GenerateGameMap(int size)
        {
            return _systemUnderTest.GenerateGameMap(new GameConfiguration()
            {
                GenerationsPerRound = 10,
                MapHeight = size,
                MapWidth = size
            });
        }


        private GameMap CreateValidGameState()
        {
            var gameMap = GenerateGameMap(10);

            for (var i = 0; i < _playerNumber; i++)
            {
                var coordinates = new List<Coordinate>
                {
                    new Coordinate()
                    {
                        X = i,
                        Y = i
                    }
                };
                var startAttributes = GenerateStartAttributes();

                _systemUnderTest.AddPlayer(new PlayerConfiguration()
                {
                    Coordinates = coordinates,
                    StartAttributes = startAttributes,
                    Player = new Player()
                    {
                        Name = $"{i}"
                    }
                });
            }

            _systemUnderTest.Start();

            Assert.That(gameMap.Tiles[2][2],
                Has.Property(nameof(Tile.Entity)).Property(nameof(Entity.Owner)).Property(nameof(Player.Name)).EqualTo("2"));
            return gameMap;
        }


    }
}