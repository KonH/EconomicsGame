using System.Collections.Generic;
using System.Linq;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using UniRx;

namespace EconomicsGame.UnityComponents {
	sealed class InventoryView : StartupInitializer {
		readonly Dictionary<int, ItemView> _items = new Dictionary<int, ItemView>();
		readonly Stack<ItemView> _itemPool = new Stack<ItemView>();

		RuntimeData _runtimeData;
		GlobalData _globalData;
		SceneData _sceneData;
		EcsEntity _playerEntity;
		CompositeDisposable _disposable;

		[SerializeField] Transform _parent;

		public override void Attach(IEcsStartup startup) {
			_runtimeData = startup.RuntimeData;
			_globalData = startup.GlobalData;
			_sceneData = startup.SceneData;
		}

		void Start() {
			_runtimeData.SelectedCharacter.Subscribe(OnSelectedCharacterChanged);
		}

		void OnSelectedCharacterChanged(EcsEntity entity) {
			var isAlive = entity.IsAlive();
			gameObject.SetActive(isAlive);
			if ( isAlive && entity.Has<Inventory>() ) {
				Init(entity, ref entity.Get<Inventory>());
			} else {
				DeInit();
			}
		}

		void Init(EcsEntity playerEntity, ref Inventory inventory) {
			DeInit();
			_playerEntity = playerEntity;
			_disposable = new CompositeDisposable();
			foreach ( var item in inventory.Items ) {
				OnAdd(ConvertIdToEntity(item));
			}
			inventory.Items
				.ObserveAdd()
				.Select(e => ConvertIdToEntity(e.Value))
				.Subscribe(OnAdd)
				.AddTo(_disposable);
			inventory.Items
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
			var instance = GetOrCreateItem();
			var isUsableItem = entity.Has<FoodItem>();
			var isPlayer = _playerEntity.Has<PlayerCharacterFlag>();
			var isCash = _runtimeData.CashService.IsCash(ref item);
			instance.Init(_sceneData, entity, ref item, isUsableItem, isUsableItem && isPlayer, isPlayer && !isCash);
			_items[item.Id] = instance;
		}

		ItemView GetOrCreateItem() {
			if ( _itemPool.Count > 0 ) {
				return _itemPool.Pop();
			}
			return Instantiate(_globalData.ItemViewPrefab, _parent);
		}

		void OnRemove(int id) {
			if ( _items.TryGetValue(id, out var item) ) {
				Remove(id, item);
			}
		}

		void Remove(int id, ItemView itemView) {
			itemView.DeInit();
			_items.Remove(id);
			_itemPool.Push(itemView);
		}
	}
}