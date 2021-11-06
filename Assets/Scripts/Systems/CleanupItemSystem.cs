using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class CleanupItemSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, EmptyItemFlag> _itemFilter;

		public void Run() {
			var itemProvider = _runtimeData.ItemProvider;
			var characterProvider = _runtimeData.CharacterProvider;
			var locationProvider = _runtimeData.LocationProvider;
			foreach ( var itemIdx in _itemFilter ) {
				ref var itemEntity = ref _itemFilter.GetEntity(itemIdx);
				ref var item = ref _itemFilter.Get1(itemIdx);
				Assert.IsTrue(characterProvider.TryGetEntity(item.Owner, out var characterEntity));
				if ( itemEntity.Has<Trade>() ) {
					ref var trade = ref itemEntity.Get<Trade>();
					Assert.IsTrue(locationProvider.TryGetComponent(trade.Location, out var location));
					location.Trades.Remove(item.Id);
				} else {
					ref var inventory = ref characterEntity.Get<Inventory>();
					inventory.Items.Remove(item.Id);
				}
				itemProvider.Remove(item.Id);
			}
		}
	}
}