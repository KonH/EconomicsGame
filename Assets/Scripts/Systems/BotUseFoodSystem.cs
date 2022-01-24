using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class BotUseFoodSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, CharacterStats, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		ItemService _itemService;

		public void Init() {
			_itemService = _runtimeData.ItemService;
		}

		public void Run() {
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
				foreach ( var itemId in inventory.Items ) {
					var itemEntity = _itemService.GetEntity(itemId);
					ref var item = ref itemEntity.Get<Item>();
					if ( item.Name != "Food" ) {
						continue;
					}
					itemEntity.Get<UseItemEvent>();
					break;
				}
			}
		}
	}
}