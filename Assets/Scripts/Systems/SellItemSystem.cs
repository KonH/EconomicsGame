using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Systems {
	public class SellItemSystem : IEcsRunSystem {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, SellItemEvent> _filter;

		public void Run() {
			var idFactory = _runtimeData.IdFactory;
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
				var tradeEntity = itemEntity.Copy();
				ref var tradeItem = ref tradeEntity.Get<Item>();
				tradeItem.Id = idFactory.GenerateNewId<Item>();
				tradeItem.Count = new ReactiveProperty<long>(sellCount);
				ref var trade = ref tradeEntity.Get<Trade>();
				ref var character = ref characterService.GetEntity(itemToSell.Owner).Get<Character>();
				trade.Location = character.CurrentLocation;
				trade.PricePerUnit = pricePerUnit;
				itemService.Add(tradeItem.Id, tradeEntity);
				ref var location = ref locationService.GetEntity(character.CurrentLocation).Get<Location>();
				location.Trades.Add(tradeItem.Id);
				if ( itemToSell.Count.Value == 0 ) {
					itemEntity.Get<EmptyItemFlag>();
				}
				Debug.Log($"Character {character.Name} sell {tradeItem.Name} x{tradeItem.Count} by {pricePerUnit}/unit");
			}
		}
	}
}