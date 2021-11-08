using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class BotUseFoodSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, CharacterStats, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		public void Run() {
			var itemService = _runtimeData.ItemService;
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
					var itemEntity = itemService.GetEntity(itemId);
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