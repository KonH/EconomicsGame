using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class BotUseFoodSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, CharacterStats, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		public void Run() {
			var itemProvider = _runtimeData.ItemProvider;
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
					Assert.IsTrue(itemProvider.TryGetEntity(itemId, out var itemEntity));
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