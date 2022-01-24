using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	// TODO: safety from double buy issue
	public class BuyItemSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, Trade, BuyItemEvent> _filter;

		CharacterService _characterService;
		ItemService _itemService;
		CashService _cashService;

		public void Init() {
			_characterService = _runtimeData.CharacterService;
			_itemService = _runtimeData.ItemService;
			_cashService = _runtimeData.CashService;
		}

		public void Run() {
			foreach ( var itemIdx in _filter ) {
				ref var itemEntity = ref _filter.GetEntity(itemIdx);
				ref var itemToBuy = ref _filter.Get1(itemIdx);
				ref var trade = ref _filter.Get2(itemIdx);
				ref var buyEvent = ref _filter.Get3(itemIdx);
				var buyCount = buyEvent.Count;
				var totalPrice = trade.PricePerUnit * buyCount;
				var buyer = buyEvent.Buyer;
				var buyerCharacterEntity = _characterService.GetEntity(buyer);
				if ( !_cashService.TryAddCash(ref buyerCharacterEntity, -totalPrice) ) {
					continue;
				}
				itemToBuy.Count.Value -= buyCount;
				itemEntity.Del<BuyItemEvent>();
				ref var buyerCharacter = ref buyerCharacterEntity.Get<Character>();
				ref var buyerInventory = ref buyerCharacterEntity.Get<Inventory>();
				var boughtItemEntity = _itemService.CreateNewItemInInventoryByCopy(ref buyerCharacter, ref buyerInventory, itemEntity);
				boughtItemEntity.Del<Trade>();
				_itemService.InitCount(boughtItemEntity, buyCount);
				var sellerCharacter = _characterService.GetEntity(itemToBuy.Owner);
				_cashService.TryAddCash(ref sellerCharacter, totalPrice);
				_itemService.TryConsume(ref itemEntity, ref itemToBuy);
				Debug.Log($"{buyerCharacterEntity.Get<Character>().Log()} bought {boughtItemEntity.Get<Item>().Log()} x{buyCount} for {totalPrice}");
			}
		}
	}
}