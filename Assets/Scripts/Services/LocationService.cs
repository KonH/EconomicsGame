using EconomicsGame.Components;
using Leopotam.Ecs;
using UniRx;

namespace EconomicsGame.Services {
	public sealed class LocationService {
		readonly ReactiveCollection<EcsEntity> _locations = new ReactiveCollection<EcsEntity>();
		readonly EntityProvider _entityProvider;

		public IReadOnlyReactiveCollection<EcsEntity> Locations => _locations;

		public LocationService(EntityProvider entityProvider) {
			_entityProvider = entityProvider;
		}

		public void Add(int id, EcsEntity entity) {
			_entityProvider.Assign<Location>(id, entity);
			_locations.Add(entity);
		}

		public EcsEntity GetEntity(int id) => _entityProvider.GetEntity<Location>(id);
	}
}