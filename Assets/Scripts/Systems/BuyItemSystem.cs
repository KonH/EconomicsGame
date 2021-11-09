using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	// TODO: safety from double buy issue
	public class BuyItemSystem : IEcsRunSystem {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, Trade, BuyItemEvent> _filter;

		public void Run() {
			// TODO: handle deps in all systems properly?
			var characterService = _runtimeData.CharacterService;
			var itemService = _runtimeData.ItemService;
			foreach ( var itemIdx in _filter ) {
				ref var itemEntity = ref _filter.GetEntity(itemIdx);
				ref var itemToBuy = ref _filter.Get1(itemIdx);
				ref var trade = ref _filter.Get2(itemIdx);
				ref var buyEvent = ref _filter.Get3(itemIdx);
				var buyCount = buyEvent.Count;
				var totalPrice = trade.PricePerUnit * buyCount;
				var buyer = buyEvent.Buyer;
				var buyerCharacterEntity = characterService.GetEntity(buyer);
				if ( !TryChangeCash(itemService, ref buyerCharacterEntity, -totalPrice) ) {
					continue;
				}
				itemToBuy.Count.Value -= buyCount;
				itemEntity.Del<BuyItemEvent>();
				ref var buyerCharacter = ref buyerCharacterEntity.Get<Character>();
				ref var buyerInventory = ref buyerCharacterEntity.Get<Inventory>();
				var boughtItemEntity = itemService.CreateNewItemInInventoryByCopy(ref buyerCharacter, ref buyerInventory, itemEntity);
				boughtItemEntity.Del<Trade>();
				itemService.InitCount(boughtItemEntity, buyCount);
				var sellerCharacter = characterService.GetEntity(itemToBuy.Owner);
				TryChangeCash(itemService, ref sellerCharacter, totalPrice);
				if ( itemToBuy.Count.Value == 0 ) {
					// TODO: handle it in one service
					itemEntity.Get<EmptyItemFlag>();
				}
				Debug.Log($"{buyerCharacterEntity.Get<Character>().Log()} bought {boughtItemEntity.Get<Item>().Log()} x{buyCount} for {totalPrice}");
			}
		}

		bool TryChangeCash(ItemService itemService, ref EcsEntity characterEntity, double addValue) {
			ref var inventory = ref characterEntity.Get<Inventory>();
			foreach ( var itemId in inventory.Items ) {
				var itemEntity = itemService.GetEntity(itemId);
				ref var item = ref itemEntity.Get<Item>();
				if ( item.Name != "Cash" ) {
					continue;
				}
				// TODO: rework checks
				var newCount = item.Count.Value + addValue;
				if ( newCount < 0 ) {
					return false;
				}
				if ( newCount == 0 ) {
					itemEntity.Get<EmptyItemFlag>();
				}
				item.Count.Value = newCount;
				return true;
			}
			if ( addValue < 0 ) {
				return false;
			}
			if ( addValue == 0 ) {
				return true;
			}
			ref var character = ref characterEntity.Get<Character>();
			var newItemEntity = itemService.CreateNewItemInInventory(ref character, ref inventory);
			itemService.InitCashItem(newItemEntity, addValue);
			return true;
		}
	}
}