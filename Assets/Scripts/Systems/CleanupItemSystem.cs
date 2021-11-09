using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class CleanupItemSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, EmptyItemFlag> _itemFilter;

		public void Run() {
			var characterService = _runtimeData.CharacterService;
			var locationService = _runtimeData.LocationService;
			var itemService = _runtimeData.ItemService;
			foreach ( var itemIdx in _itemFilter ) {
				ref var itemEntity = ref _itemFilter.GetEntity(itemIdx);
				ref var item = ref _itemFilter.Get1(itemIdx);
				var characterEntity = characterService.GetEntity(item.Owner);
				if ( itemEntity.Has<Trade>() ) {
					ref var trade = ref itemEntity.Get<Trade>();
					ref var location = ref locationService.GetEntity(trade.Location).Get<Location>();
					itemService.RemoveTradeFromLocation(item.Id, ref location);
					Debug.Log($"Item {item.Log()} removed from location {location.Log()} trades");
				} else {
					ref var inventory = ref characterEntity.Get<Inventory>();
					itemService.RemoveFromInventory(item.Id, ref inventory);
					Debug.Log($"Item {item.Log()} removed from {characterEntity.Get<Character>().Log()} inventory");
				}
			}
		}
	}
}