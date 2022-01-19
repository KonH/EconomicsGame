using System.Reflection;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	sealed class WorldLoader {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;

		public WorldLoader(EcsWorld world, RuntimeData runtimeData) {
			_world = world;
			_runtimeData = runtimeData;
		}

		public bool TryLoad() {
			if ( !_runtimeData.PersistantService.CanLoad ) {
				return false;
			}
			var data = _runtimeData.PersistantService.Load();
			var copyComponentDataGenericMethod = typeof(WorldLoader).GetMethod(nameof(CopyComponentData), BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.IsNotNull(copyComponentDataGenericMethod);
			foreach ( var entityData in data ) {
				var entity = _world.NewEntity();
				foreach ( var componentData in entityData ) {
					var componentType = componentData.GetType();
					var copyComponentDataMethod = copyComponentDataGenericMethod.MakeGenericMethod(componentType);
					copyComponentDataMethod.Invoke(this, new [] { entity, componentData });
					if ( componentType == typeof(Location) ) {
						ref var location = ref entity.Get<Location>();
						_runtimeData.IdFactory.AdvanceTo<Location>(location.Id);
						_runtimeData.LocationService.AddInitializedLocation(location.Id, entity);
					} else if ( componentType == typeof(Character) ) {
						ref var character = ref entity.Get<Character>();
						_runtimeData.IdFactory.AdvanceTo<Character>(character.Id);
						_runtimeData.CharacterService.AddInitializedCharacter(character.Id, entity);
					} else if ( componentType == typeof(Item) ) {
						ref var item = ref entity.Get<Item>();
						_runtimeData.IdFactory.AdvanceTo<Item>(item.Id);
						_runtimeData.ItemService.AddInitializedItem(item.Id, entity);
					} else if ( componentType == typeof(Market) ) {
						_runtimeData.MarketService.AddMarket(entity);
					}
				}
			}
			return true;
		}

		void CopyComponentData<T>(EcsEntity entity, T data) where T : struct {
			ref var component = ref entity.Get<T>();
			component = data;
		}
	}
}