using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class UseFoodSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, FoodItem, UseItemEvent> _itemFilter;

		CharacterService _characterService;
		ItemService _itemService;

		public void Init() {
			_characterService = _runtimeData.CharacterService;
			_itemService = _runtimeData.ItemService;
		}

		public void Run() {
			foreach ( var itemIdx in _itemFilter ) {
				ref var item = ref _itemFilter.Get1(itemIdx);
				ref var foodItem = ref _itemFilter.Get2(itemIdx);
				var characterEntity = _characterService.GetEntity(item.Owner);
				if ( !characterEntity.Has<CharacterStats>() ) {
					continue;
				}
				ref var stats = ref characterEntity.Get<CharacterStats>();
				if ( !stats.Values.TryGetValue("Hunger", out var hunger) ) {
					continue;
				}
				hunger.Value = Mathf.Clamp01(hunger.Value - foodItem.Restore);
				item.Count.Value -= 1;
				var remainingCount = item.Count.Value;
				Debug.Log($"Character {characterEntity.Get<Character>().Log()} consumes food item {item.Log()}, {remainingCount} remaining");
				ref var entity = ref _itemFilter.GetEntity(itemIdx);
				_itemService.TryConsume(ref entity, ref item);
			}
		}
	}
}