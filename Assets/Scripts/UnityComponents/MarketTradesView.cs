using System.Collections.Generic;
using System.Linq;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.UnityComponents {
	sealed class MarketTradesView : StartupInitializer, IPostInitializer {
		readonly Dictionary<int, TradeView> _items = new Dictionary<int, TradeView>();
		readonly Stack<TradeView> _itemPool = new Stack<TradeView>();

		RuntimeData _runtimeData;
		GlobalData _globalData;
		SceneData _sceneData;
		CompositeDisposable _disposable;

		[SerializeField] Transform _parent;

		public override void Attach(IEcsStartup startup) {
			_runtimeData = startup.RuntimeData;
			_globalData = startup.GlobalData;
			_sceneData = startup.SceneData;
		}

		public void PostInit() {
			ref var market = ref _runtimeData.MarketService.Market;
			Init(ref market);
		}

		void Init(ref Market market) {
			DeInit();
			_disposable = new CompositeDisposable();
			var trades = market.Trades;
			foreach ( var item in trades ) {
				OnAdd(ConvertIdToEntity(item));
			}
			trades
				.ObserveAdd()
				.Select(e => ConvertIdToEntity(e.Value))
				.Subscribe(OnAdd)
				.AddTo(_disposable);
			trades
				.ObserveRemove()
				.Subscribe(e => OnRemove(e.Value))
				.AddTo(_disposable);
		}

		void DeInit() {
			_disposable?.Dispose();
			var keys = _items.Keys.ToArray();
			foreach ( var id in keys ) {
				Remove(id, _items[id]);
			}
		}

		EcsEntity ConvertIdToEntity(int id) =>
			_runtimeData.ItemService.GetEntity(id);

		void OnAdd(EcsEntity entity) {
			ref var item = ref entity.Get<Item>();
			ref var trade = ref entity.Get<Trade>();
			var instance = GetOrCreateItem();
			var selectedCharacter = _runtimeData.SelectedCharacter.Value;
			var canBuy = selectedCharacter.IsAlive() && selectedCharacter.Has<PlayerCharacterFlag>(); // TODO: cash test, dynamic
			instance.Init(_runtimeData, _sceneData, selectedCharacter, entity, ref item, ref trade, canBuy);
			_items[item.Id] = instance;
		}

		TradeView GetOrCreateItem() {
			if ( _itemPool.Count > 0 ) {
				return _itemPool.Pop();
			}
			return Instantiate(_globalData.TradeViewPrefab, _parent);
		}

		void OnRemove(int id) {
			if ( _items.TryGetValue(id, out var item) ) {
				Remove(id, item);
			}
		}

		void Remove(int id, TradeView tradeView) {
			tradeView.DeInit();
			_items.Remove(id);
			_itemPool.Push(tradeView);
		}
	}
}