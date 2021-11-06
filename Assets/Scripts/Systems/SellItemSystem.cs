using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public class SellItemSystem : IEcsRunSystem {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Item, SellItemEvent> _filter;

		public void Run() {
			var idFactory = _runtimeData.IdFactory;
			var itemProvider = _runtimeData.ItemProvider;
			var characterProvider = _runtimeData.CharacterProvider;
			var locationProvider = _runtimeData.LocationProvider;
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
				Assert.IsTrue(characterProvider.TryGetComponent(itemToSell.Owner, out var character));
				trade.Location = character.CurrentLocation;
				trade.PricePerUnit = pricePerUnit;
				itemProvider.Assign(tradeItem.Id, tradeEntity);
				Assert.IsTrue(locationProvider.TryGetComponent(character.CurrentLocation, out var location));
				location.Trades.Add(tradeItem.Id);
				if ( itemToSell.Count.Value == 0 ) {
					itemEntity.Get<EmptyItemFlag>();
				}
				Debug.Log($"Character {character.Name} sell {tradeItem.Name} x{tradeItem.Count} by {pricePerUnit}/unit");
			}
		}
	}
}