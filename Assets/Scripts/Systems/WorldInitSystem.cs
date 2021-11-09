using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class WorldInitSystem : IEcsInitSystem {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;

		readonly Vector2[] _locations = {
			new Vector2(-1.25f, 0), new Vector2(1.25f, 0), new Vector2(1.25f, 0.5f), new Vector2(1.25f, -0.5f)
		};

		public void Init() {
			var loader = new WorldLoader(_world, _runtimeData);
			if ( !loader.TryLoad() ) {
				Generate();
			}
		}

		// TODO: to custom system/service
		void Generate() {
			var locationService = _runtimeData.LocationService;
			var characterService = _runtimeData.CharacterService;
			var itemService = _runtimeData.ItemService;
			var first = true;
			foreach ( var locationSetup in _locations ) {
				var locationEntity = locationService.CreateNewLocation(locationSetup);
				ref var location = ref locationEntity.Get<Location>();
				if ( !first ) {
					ref var source = ref locationEntity.Get<FoodSource>();
					source.Remaining = 10;
					continue;
				}
				for ( var i = 0; i < 5; i++ ) {
					var characterEntity = characterService.CreateNewCharacterInLocation(ref location);
					ref var character = ref characterEntity.Get<Character>();
					ref var inventory = ref characterEntity.Get<Inventory>();
					if ( first ) {
						characterEntity.Get<PlayerCharacterFlag>();
						first = false;
					} else {
						characterEntity.Get<BotCharacterFlag>();
					}

					var foodEntity = itemService.CreateNewItemInInventory(ref character, ref inventory);
					itemService.InitFoodItem(foodEntity, 1, 1);

					var cashEntity = itemService.CreateNewItemInInventory(ref character, ref inventory);
					itemService.InitCashItem(cashEntity, 100);

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
		}
	}
}