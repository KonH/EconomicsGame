using System;
using System.Collections.Generic;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public class SaveSystem : IEcsRunSystem {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;

		EcsEntity[] _entities;
		Type[] _componentTypes;
		object[] _componentValues;

		public void Run() {
			// TODO: trigger by event
			var entityCount = _world.GetAllEntities(ref _entities);
			var persistantEntities = new List<List<object>>(entityCount);
			foreach ( var entity in _entities ) {
				var componentCount = entity.GetComponentsCount();
				entity.GetComponentTypes(ref _componentTypes);
				entity.GetComponentValues(ref _componentValues);
				var persistantComponents = new List<object>(componentCount);
				for ( var i = 0; i < componentCount; i++ ) {
					var type = _componentTypes[i];
					var isPersistantComponent = typeof(IPersistantComponent).IsAssignableFrom(type);
					if ( !isPersistantComponent ) {
						continue;
					}
					persistantComponents.Add(_componentValues[i]);
				}
				if ( persistantComponents.Count > 0 ) {
					persistantEntities.Add(persistantComponents);
				}
			}
			_runtimeData.PersistantService.Save(persistantEntities);
		}
	}
}