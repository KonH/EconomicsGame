using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class BotSellFoodSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		public void Run() {
			var itemProvider = _runtimeData.ItemProvider;
			var locationProvider = _runtimeData.LocationProvider;
			foreach ( var characterIdx in _filter ) {
				ref var character = ref _filter.Get1(characterIdx);
				Assert.IsTrue(locationProvider.TryGetEntity(character.CurrentLocation, out var currentLocationEntity));
				if ( currentLocationEntity.Has<FoodSource>() ) {
					continue;
				}
				var shouldSellFood = false;
				var totalFoodCount = 0L;
				var lastFoodEntity = default(EcsEntity);
				ref var inventory = ref _filter.Get3(characterIdx);
				foreach ( var itemId in inventory.Items ) {
					Assert.IsTrue(itemProvider.TryGetEntity(itemId, out var itemEntity));
					ref var item = ref itemEntity.Get<Item>();
					if ( item.Name != "Food" ) {
						continue;
					}
					totalFoodCount += item.Count.Value;
					if ( totalFoodCount <= 2 ) {
						continue;
					}
					shouldSellFood = true;
					lastFoodEntity = itemEntity;
					break;
				}
				if ( !shouldSellFood ) {
					continue;
				}
				ref var sellEvent = ref lastFoodEntity.Get<SellItemEvent>();
				sellEvent.Count = 1;
				sellEvent.PricePerUnit = 1;
			}
		}
	}
}