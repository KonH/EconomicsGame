using System.Collections.Generic;
using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Services {
	public sealed class ComponentProvider<T> where T : struct, IIdOwner {
		readonly Dictionary<int, EcsEntity> _entities = new Dictionary<int, EcsEntity>();

		public void Assign(int id, EcsEntity entity) {
			_entities[id] = entity;
		}

		// TODO: no try method with internal assert
		public bool TryGetEntity(int id, out EcsEntity entity) =>
			_entities.TryGetValue(id, out entity);

		// TODO: no try method with internal assert
		public bool TryGetComponent(int id, out T component) {
			component = default;
			if ( TryGetEntity(id, out var entity) ) {
				component = entity.Get<T>();
				return true;
			}
			return false;
		}

		public void Remove(int id) {
			_entities.Remove(id);
		}
	}
}