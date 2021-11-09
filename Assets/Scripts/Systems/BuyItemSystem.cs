using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Systems {
	// TODO: safety from double buy issue
	public class BuyItemSystem : IEcsRunSystem {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, Trade, BuyItemEvent> _filter;

		public void Run() {
			// TODO: handle deps in all systems properly?
			var idFactory = _runtimeData.IdFactory;
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
				if ( !TryChangeCash(idFactory, itemService, ref buyerCharacterEntity, -totalPrice) ) {
					continue;
				}
				itemToBuy.Count.Value -= buyCount;
				itemEntity.Del<BuyItemEvent>();
				var boughtItemEntity = itemEntity.Copy();
				boughtItemEntity.Del<Trade>();
				ref var boughtItem = ref boughtItemEntity.Get<Item>();
				boughtItem.Id = idFactory.GenerateNewId<Item>();
				boughtItem.Owner = buyer;
				boughtItem.Count = new ReactiveProperty<double>(buyCount);
				var sellerCharacter = characterService.GetEntity(itemToBuy.Owner);
				TryChangeCash(idFactory, itemService, ref sellerCharacter, totalPrice);
				itemService.AddToInventory(boughtItem.Id, boughtItemEntity, ref buyerCharacterEntity.Get<Inventory>());
				if ( itemToBuy.Count.Value == 0 ) {
					// TODO: handle it in one service
					itemEntity.Get<EmptyItemFlag>();
				}
				Debug.Log($"{buyerCharacterEntity.Get<Character>().Log()} bought {boughtItem.Log()} x{buyCount} for {totalPrice}");
			}
		}

		bool TryChangeCash(IdFactory idFactory, ItemService itemService, ref EcsEntity characterEntity, long addValue) {
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
			var newItemEntity = _world.NewEntity();
			ref var newItem = ref newItemEntity.Get<Item>();
			newItem.Id = idFactory.GenerateNewId<Item>();
			ref var character = ref characterEntity.Get<Character>();
			newItem.Owner = character.Id;
			newItem.Name = "Cash";
			newItem.Count = new ReactiveProperty<double>(addValue);
			itemService.AddToInventory(newItem.Id, newItemEntity, ref inventory);
			return true;
		}
	}
}