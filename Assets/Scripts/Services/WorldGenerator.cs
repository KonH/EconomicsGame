using EconomicsGame.Components;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Services {
	public sealed class WorldGenerator {
		readonly Vector2[] _locations = {
			new Vector2(-1.25f, 0), new Vector2(1.25f, 0), new Vector2(1.25f, 0.5f), new Vector2(1.25f, -0.5f)
		};

		readonly LocationService _locationService;
		readonly CharacterService _characterService;
		readonly ItemService _itemService;
		readonly MarketService _marketService;

		public WorldGenerator(RuntimeData runtimeData) {
			_locationService = runtimeData.LocationService;
			_characterService = runtimeData.CharacterService;
			_itemService = runtimeData.ItemService;
			_marketService = runtimeData.MarketService;
		}

		public void Generate() {
			var isFirst = true;
			foreach ( var locationSetup in _locations ) {
				var locationEntity = _locationService.CreateNewLocation(locationSetup);
				ref var location = ref locationEntity.Get<Location>();
				if ( !isFirst ) {
					ref var source = ref locationEntity.Get<FoodSource>();
					source.Remaining = 10;
					continue;
				}
				for ( var i = 0; i < 5; i++ ) {
					var characterEntity = _characterService.CreateNewCharacterInLocation(ref location);
					ref var character = ref characterEntity.Get<Character>();
					ref var inventory = ref characterEntity.Get<Inventory>();
					if ( isFirst ) {
						characterEntity.Get<PlayerCharacterFlag>();
						isFirst = false;
					} else {
						characterEntity.Get<BotCharacterFlag>();
					}

					_itemService.CreateNewItemInInventory(
						ref character, ref inventory,
						e => _itemService.InitFoodItem(e, 1, 1));

					_itemService.CreateNewItemInInventory(
						ref character, ref inventory,
						e => _itemService.InitCashItem(e, 100));

					ref var stats = ref characterEntity.Get<CharacterStats>();
					switch ( i ) {
						case 1:
							stats.Values.Add("Brave", new ReactiveProperty<float>(0));
							break;
						case 2:
							stats.Values.Add("Coward", new ReactiveProperty<float>(0));
							break;
					}
				}
			}
			_marketService.CreateMarket();
		}
	}
}