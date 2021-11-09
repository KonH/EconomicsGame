using System;
using System.Collections.Generic;
using EconomicsGame.Components;
using Leopotam.Ecs;
using UnityEngine.Assertions;

namespace EconomicsGame.Services {
	public sealed class EntityProvider {
		readonly Dictionary<Type, Dictionary<int, EcsEntity>> _entities = new Dictionary<Type, Dictionary<int, EcsEntity>>();

		public void Assign<T>(int id, EcsEntity entity) where T : struct {
			var type = typeof(T);
			if ( !_entities.TryGetValue(type, out var storage) ) {
				storage = new Dictionary<int, EcsEntity>();
				_entities[type] = storage;
			}
			Assert.IsFalse(storage.ContainsKey(id));
			storage[id] = entity;
		}

		public EcsEntity GetEntity<T>(int id) where T : struct {
			Assert.IsTrue(TryGetEntity<T>(id, out var entity), $"Failed to find entity of type {typeof(T)} with ID {id}");
			return entity;
		}

		public bool TryGetEntity<T>(int id, out EcsEntity entity) where T : struct {
			entity = default;
			return _entities.TryGetValue(typeof(T), out var storage) && storage.TryGetValue(id, out entity);
		}

		public void Remove<T>(int id) {
			if ( _entities.TryGetValue(typeof(T), out var storage) ) {
				storage.Remove(id);
			}
		}
	}
}