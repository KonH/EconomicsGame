using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public class SellItemSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, SellItemEvent> _filter;

		public void Run() {
			var characterService = _runtimeData.CharacterService;
			var itemService = _runtimeData.ItemService;
			var locationService = _runtimeData.LocationService;
			foreach ( var itemIdx in _filter ) {
				ref var itemEntity = ref _filter.GetEntity(itemIdx);
				ref var itemToSell = ref _filter.Get1(itemIdx);
				ref var sellEvent = ref _filter.Get2(itemIdx);
				var pricePerUnit = sellEvent.PricePerUnit;
				var sellCount = sellEvent.Count;
				itemToSell.Count.Value -= sellCount;
				itemEntity.Del<SellItemEvent>();
				ref var character = ref characterService.GetEntity(itemToSell.Owner).Get<Character>();
				ref var location = ref locationService.GetEntity(character.CurrentLocation).Get<Location>();
				var tradeEntity = itemService.CreateTradeAtLocation(ref character, ref location, itemEntity, sellCount, pricePerUnit);
				if ( itemToSell.Count.Value == 0 ) {
					itemEntity.Get<EmptyItemFlag>();
				}
				ref var tradeItem = ref tradeEntity.Get<Item>();
				Debug.Log($"Character {character.Log()} sell {tradeItem.Log()} x{tradeItem.Count} by {pricePerUnit}/unit");
			}
		}
	}
}