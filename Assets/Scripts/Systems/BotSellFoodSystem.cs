using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class BotSellFoodSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		LocationService _locationService;
		ItemService _itemService;

		public void Init() {
			_locationService = _runtimeData.LocationService;
			_itemService = _runtimeData.ItemService;
		}

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				ref var character = ref _filter.Get1(characterIdx);
				var currentLocationEntity = _locationService.GetEntity(character.CurrentLocation);
				if ( currentLocationEntity.Has<FoodSource>() ) {
					continue;
				}
				var shouldSellFood = false;
				var totalFoodCount = 0.0;
				var lastFoodEntity = default(EcsEntity);
				ref var inventory = ref _filter.Get3(characterIdx);
				foreach ( var itemId in inventory.Items ) {
					var itemEntity = _itemService.GetEntity(itemId);
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