using EconomicsGame.Components;
using Leopotam.Ecs;
using UniRx;
using UnityEngine.Assertions;

namespace EconomicsGame.Services {
	public sealed class MarketService {
		public ref Market Market {
			get {
				Assert.IsTrue(_marketEntity.IsAlive());
				return ref _marketEntity.Get<Market>();
			}
		}

		readonly EcsWorld _world;

		EcsEntity _marketEntity;

		public MarketService(EcsWorld world) {
			_world = world;
		}

		public EcsEntity CreateMarket() {
			var entity = _world.NewEntity();
			ref var market = ref entity.Get<Market>();
			market.Trades = new ReactiveCollection<int>();
			_marketEntity = entity;
			return entity;
		}

		public void AddMarket(EcsEntity entity) {
			_marketEntity = entity;
		}
	}
}