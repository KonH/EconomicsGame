using EconomicsGame.Components;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Services {
	public sealed class LocationService {
		readonly ReactiveCollection<EcsEntity> _locations = new ReactiveCollection<EcsEntity>();

		readonly EcsWorld _world;
		readonly IdFactory _idFactory;
		readonly EntityProvider _entityProvider;

		public IReadOnlyReactiveCollection<EcsEntity> Locations => _locations;

		public LocationService(EcsWorld world, IdFactory idFactory, EntityProvider entityProvider) {
			_world = world;
			_idFactory = idFactory;
			_entityProvider = entityProvider;
		}

		public void AddInitializedLocation(int id, EcsEntity entity) {
			_entityProvider.Assign<Location>(id, entity);
			_locations.Add(entity);
		}

		public EcsEntity CreateNewLocation(Vector2 position) {
			var entity = _world.NewEntity();
			ref var location = ref entity.Get<Location>();
			location.Id = _idFactory.GenerateNewId<Location>();
			location.Name = $"Location {location.Id.ToString()}";
			location.Position = position;
			location.Characters = new ReactiveCollection<int>();
			location.Trades = new ReactiveCollection<int>();
			AddInitializedLocation(location.Id, entity);
			return entity;
		}

		public EcsEntity GetEntity(int id) => _entityProvider.GetEntity<Location>(id);
	}
}