using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	// TODO: safety from double buy issue
	public class BuyItemSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, Trade, BuyItemEvent> _filter;

		public void Run() {
			// TODO: handle deps in all systems properly?
			var characterService = _runtimeData.CharacterService;
			var itemService = _runtimeData.ItemService;
			var cashService = _runtimeData.CashService;
			foreach ( var itemIdx in _filter ) {
				ref var itemEntity = ref _filter.GetEntity(itemIdx);
				ref var itemToBuy = ref _filter.Get1(itemIdx);
				ref var trade = ref _filter.Get2(itemIdx);
				ref var buyEvent = ref _filter.Get3(itemIdx);
				var buyCount = buyEvent.Count;
				var totalPrice = trade.PricePerUnit * buyCount;
				var buyer = buyEvent.Buyer;
				var buyerCharacterEntity = characterService.GetEntity(buyer);
				if ( !cashService.TryAddCash(ref buyerCharacterEntity, -totalPrice) ) {
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
				cashService.TryAddCash(ref sellerCharacter, totalPrice);
				itemService.TryConsume(ref itemEntity, ref itemToBuy);
				Debug.Log($"{buyerCharacterEntity.Get<Character>().Log()} bought {boughtItemEntity.Get<Item>().Log()} x{buyCount} for {totalPrice}");
			}
		}
	}
}