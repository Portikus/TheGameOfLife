using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameOfLife.Api;
using GameOfLife.Api.Model;
using GameOfLife.Frontend.Wpf.Model;
using Prism.Mvvm;

namespace GameOfLife.Frontend.Wpf.ViewModels
{
    public class EntityAttributesViewModel : BindableBase
    {
        public Dictionary<EntityAttribute, int> EntityAttributes { get; private set; }

        private readonly PlayerProvider _playerProvider;
        private readonly IGameManager _gameManager;

        public EntityAttributesViewModel(PlayerProvider playerProvider, IGameManager gameManager)
        {
            _playerProvider = playerProvider;
            _gameManager = gameManager;

            EntityAttributes = new Dictionary<EntityAttribute, int>();

            playerProvider.PropertyChanged += PlayerProviderOnPropertyChanged;
        }

        private void PlayerProviderOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(_playerProvider.CurrentPlayer))
            {
                UpdateCurrentEntityAttributes();
            }
        }

        private void UpdateCurrentEntityAttributes()
        {
            EntityAttributes.Clear();
            var entity = FindEntityOfCurrentUser();
            if (entity != null)
            {
                FillEntityAttributes(entity);
            }
        }

        private Entity FindEntityOfCurrentUser()
        {
            var gameMap = _gameManager.GameMap;
            var currentPlayer = _playerProvider.CurrentPlayer;
            for (int i = 0; i < gameMap.Tiles.Length; i++)
            {
                for (int j = 0; j < gameMap.Tiles[i].Length; j++)
                {
                    var currentEntity = gameMap.Tiles[i][j].Entity;
                    if (currentEntity.Owner.Name == currentPlayer.Name)
                    {
                        return currentEntity;
                    };
                }
            }
            return null;
        }

        private void FillEntityAttributes(Entity entity)
        {
            foreach (var entityAttributesKey in entity.EntityAttributes.Keys)
            {
                EntityAttributes.Add(entityAttributesKey, EntityAttributes[entityAttributesKey]);
            }
        }
    }
}