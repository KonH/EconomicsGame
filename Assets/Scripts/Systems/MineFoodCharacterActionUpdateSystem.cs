using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using LocationService = EconomicsGame.Services.LocationService;

namespace EconomicsGame.Systems {
	public sealed class MineFoodCharacterActionUpdateSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, MineFoodCharacterAction, CharacterActionProgress, Inventory>.Exclude<DeadCharacterFlag> _filter;

		ItemService _itemService;
		LocationService _locationService;

		public void Init() {
			_itemService = _runtimeData.ItemService;
			_locationService = _runtimeData.LocationService;
		}

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var mineFoodAction = ref _filter.Get2(characterIdx);
				ref var actionProgress = ref _filter.Get3(characterIdx);
				var isCompleted = actionProgress.Progress >= 1;
				if ( !isCompleted ) {
					continue;
				}
				ref var inventory = ref _filter.Get4(characterIdx);
				var shouldAddItem = true;
				foreach ( var itemId in inventory.Items ) {
					ref var item = ref _itemService.GetEntity(itemId).Get<Item>();
					if ( item.Name != "Food" ) {
						continue;
					}
					item.Count.Value += 1;
					shouldAddItem = false;
					break;
				}
				ref var character = ref _filter.Get1(characterIdx);
				if ( shouldAddItem ) {
					var itemEntity = _itemService.CreateNewItemInInventory(ref character, ref inventory);
					_itemService.InitFoodItem(itemEntity, 1, 1);
				}
				var locationEntity = _locationService.GetEntity(mineFoodAction.TargetLocation);
				ref var source = ref locationEntity.Get<FoodSource>();
				source.Locked--;
				source.Remaining--;
				characterEntity.Del<MineFoodCharacterAction>();
				characterEntity.Del<BusyCharacterFlag>();
				Debug.Log($"Character {character.Log()} completed mining food at {locationEntity.Get<Location>().Log()}");
			}
		}
	}
}