using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class BotBuyFoodSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, CharacterStats, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		public void Run() {
			var itemService = _runtimeData.ItemService;
			var marketService = _runtimeData.MarketService;
			var cashService = _runtimeData.CashService;
			foreach ( var characterIdx in _filter ) {
				ref var stats = ref _filter.Get3(characterIdx);
				if ( !stats.Values.TryGetValue("Hunger", out var hunger) ) {
					continue;
				}
				var shouldEat = hunger.Value > 0.5f;
				if ( !shouldEat ) {
					continue;
				}
				ref var inventory = ref _filter.Get4(characterIdx);
				var hasFood = false;
				foreach ( var itemId in inventory.Items ) {
					ref var item = ref itemService.GetEntity(itemId).Get<Item>();
					if ( item.Name != "Food" ) {
						continue;
					}
					hasFood = true;
					break;
				}
				if ( hasFood ) {
					continue;
				}
				var currentCash = cashService.GetCurrentCash(ref _filter.GetEntity(characterIdx));
				ref var market = ref marketService.Market;
				ref var character = ref _filter.Get1(characterIdx);
				foreach ( var tradeId in market.Trades ) {
					var itemEntity = itemService.GetEntity(tradeId);
					ref var item = ref itemEntity.Get<Item>();
					if ( item.Name != "Food" ) {
						continue;
					}
					var trade = itemEntity.Get<Trade>();
					if ( trade.PricePerUnit > currentCash ) {
						continue;
					}
					ref var buyEvent = ref itemEntity.Get<BuyItemEvent>();
					buyEvent.Buyer = character.Id;
					buyEvent.Count = 1;
					break;
				}
			}
		}
	}
}