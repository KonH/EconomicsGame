using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class BotBuyFoodSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, CharacterStats, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		public void Run() {
			var itemProvider = _runtimeData.ItemProvider;
			var locationProvider = _runtimeData.LocationProvider;
			foreach ( var characterIdx in _filter ) {
				ref var stats = ref _filter.Get3(characterIdx);
				if ( !stats.Values.TryGetValue("Hunger", out var hunger) ) {
					continue;
				}
				var shouldEat = hunger.Value > 0.5f;
				if ( !shouldEat ) {
					continue;
				}
				ref var inventory = ref _filter.Get4(characterIdx);
				var hasFood = false;
				foreach ( var itemId in inventory.Items ) {
					Assert.IsTrue(itemProvider.TryGetEntity(itemId, out var itemEntity));
					ref var item = ref itemEntity.Get<Item>();
					if ( item.Name != "Food" ) {
						continue;
					}
					hasFood = true;
					break;
				}
				if ( hasFood ) {
					continue;
				}
				ref var character = ref _filter.Get1(characterIdx);
				Assert.IsTrue(locationProvider.TryGetEntity(character.CurrentLocation, out var currentLocationEntity));
				ref var currentLocation = ref currentLocationEntity.Get<Location>();
				// TODO: check funds
				foreach ( var tradeId in currentLocation.Trades ) {
					Assert.IsTrue(itemProvider.TryGetEntity(tradeId, out var itemEntity));
					ref var item = ref itemEntity.Get<Item>();
					if ( item.Name != "Food" ) {
						continue;
					}
					ref var buyEvent = ref itemEntity.Get<BuyItemEvent>();
					buyEvent.Buyer = character.Id;
					buyEvent.Count = 1;
					break;
				}
			}
		}
	}
}