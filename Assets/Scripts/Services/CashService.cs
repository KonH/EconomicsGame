using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Services {
	public sealed class CashService {
		readonly ItemService _itemService;

		public CashService(ItemService itemService) {
			_itemService = itemService;
		}

		public bool IsCash(ref Item item) => item.Name == "Cash";

		public double GetCurrentCash(ref EcsEntity characterEntity) =>
			TryGetCash(ref characterEntity, out _, out var cashItem) ? cashItem.Count.Value : 0;

		public bool TryAddCash(ref EcsEntity characterEntity, double addCash) {
			var currentCash = GetCurrentCash(ref characterEntity);
			var newCash = currentCash + addCash;
			if ( newCash < 0 ) {
				return false;
			}
			if ( !TryGetCash(ref characterEntity, out var cashEntity, out var cashItem) ) {
				ref var character = ref characterEntity.Get<Character>();
				ref var inventory = ref characterEntity.Get<Inventory>();
				cashEntity = _itemService.CreateNewItemInInventory(
					ref character, ref inventory,
					e => _itemService.InitCashItem(e, addCash));
				cashItem = cashEntity.Get<Item>();
			}
			_itemService.TryConsume(ref cashEntity, ref cashItem);
			cashItem.Count.Value = newCash;
			return true;
		}

		bool TryGetCash(ref EcsEntity characterEntity, out EcsEntity cashEntity, out Item cashComponent) {
			ref var inventory = ref characterEntity.Get<Inventory>();
			foreach ( var itemId in inventory.Items ) {
				var entity = _itemService.GetEntity(itemId);
				ref var item = ref entity.Get<Item>();
				if ( !IsCash(ref item) ) {
					continue;
				}
				cashEntity = entity;
				cashComponent = item;
				return true;
			}
			cashEntity = default;
			cashComponent = default;
			return false;
		}
	}
}