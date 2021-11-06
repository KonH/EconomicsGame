using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class UseFoodSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, FoodItem, UseItemEvent> _itemFilter;

		public void Run() {
			var characterProvider = _runtimeData.CharacterProvider;
			foreach ( var itemIdx in _itemFilter ) {
				ref var item = ref _itemFilter.Get1(itemIdx);
				ref var foodItem = ref _itemFilter.Get2(itemIdx);
				if ( !characterProvider.TryGetEntity(item.Owner, out var characterEntity) || !characterEntity.Has<CharacterStats>() ) {
					continue;
				}
				ref var stats = ref characterEntity.Get<CharacterStats>();
				if ( !stats.Values.TryGetValue("Hunger", out var hunger) ) {
					continue;
				}
				hunger.Value = Mathf.Clamp01(hunger.Value - foodItem.Restore);
				item.Count.Value -= 1;
				// TODO: fix assertion failure issue
				Debug.Log($"Character {characterEntity.Get<Character>().Name} consumes food item {item.Name}");
				if ( item.Count.Value > 0 ) {
					continue;
				}
				ref var entity = ref _itemFilter.GetEntity(itemIdx);
				entity.Get<EmptyItemFlag>();
			}
		}
	}
}