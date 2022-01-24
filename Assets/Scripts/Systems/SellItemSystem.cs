using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public class SellItemSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, SellItemEvent> _filter;

		CharacterService _characterService;
		ItemService _itemService;

		public void Init() {
			_characterService = _runtimeData.CharacterService;
			_itemService = _runtimeData.ItemService;
		}

		public void Run() {
			foreach ( var itemIdx in _filter ) {
				ref var itemEntity = ref _filter.GetEntity(itemIdx);
				ref var itemToSell = ref _filter.Get1(itemIdx);
				ref var sellEvent = ref _filter.Get2(itemIdx);
				var pricePerUnit = sellEvent.PricePerUnit;
				var sellCount = sellEvent.Count;
				itemToSell.Count.Value -= sellCount;
				itemEntity.Del<SellItemEvent>();
				ref var character = ref _characterService.GetEntity(itemToSell.Owner).Get<Character>();
				var tradeEntity = _itemService.CreateTrade(ref character, itemEntity, sellCount, pricePerUnit);
				_itemService.TryConsume(ref itemEntity, ref itemToSell);
				ref var tradeItem = ref tradeEntity.Get<Item>();
				Debug.Log($"Character {character.Log()} sell {tradeItem.Log()} x{tradeItem.Count} by {pricePerUnit}/unit");
			}
		}
	}
}