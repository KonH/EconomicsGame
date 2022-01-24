using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class CleanupItemSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, EmptyItemFlag> _itemFilter;

		CharacterService _characterService;
		ItemService _itemService;

		public void Init() {
			_characterService = _runtimeData.CharacterService;
			_itemService = _runtimeData.ItemService;
		}

		public void Run() {
			foreach ( var itemIdx in _itemFilter ) {
				ref var itemEntity = ref _itemFilter.GetEntity(itemIdx);
				ref var item = ref _itemFilter.Get1(itemIdx);
				var characterEntity = _characterService.GetEntity(item.Owner);
				if ( itemEntity.Has<Trade>() ) {
					_itemService.RemoveTrade(item.Id);
					Debug.Log($"Item {item.Log()} removed from trades");
				} else {
					ref var inventory = ref characterEntity.Get<Inventory>();
					_itemService.RemoveFromInventory(item.Id, ref inventory);
					Debug.Log($"Item {item.Log()} removed from {characterEntity.Get<Character>().Log()} inventory");
				}
			}
		}
	}
}