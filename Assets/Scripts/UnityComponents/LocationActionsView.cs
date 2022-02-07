using System;
using System.Collections.Generic;
using EconomicsGame.Components;
using EconomicsGame.Services;
using EconomicsGame.Services.ActionHandlers;
using Leopotam.Ecs;
using UnityEngine;
using UniRx;

namespace EconomicsGame.UnityComponents {
	sealed class LocationActionsView : StartupInitializer {
		readonly List<IActionHandler> _actions = new List<IActionHandler> {
			new MoveActionHandler(),
			new MineFoodActionHandler()
		};

		readonly List<LocationActionView> _items = new List<LocationActionView>();
		readonly Stack<LocationActionView> _itemPool = new Stack<LocationActionView>();

		RuntimeData _runtimeData;
		GlobalData _globalData;
		EcsEntity _playerEntity;
		EcsEntity _locationEntity;

		[SerializeField] Transform _parent;

		public override void Attach(IEcsStartup startup) {
			_runtimeData = startup.RuntimeData;
			_globalData = startup.GlobalData;
		}

		void Start() {
			_runtimeData.SelectedCharacter
				.Subscribe(OnCharacterChanged);
			_runtimeData.SelectedLocation
				.Subscribe(OnLocationChanged);
		}

		void OnCharacterChanged(EcsEntity entity) {
			_playerEntity = entity;
			OnSelectionChanged();
		}

		void OnLocationChanged(EcsEntity entity) {
			_locationEntity = entity;
			OnSelectionChanged();
		}

		void OnSelectionChanged() {
			var isAlive = _playerEntity.IsAlive() && _locationEntity.IsAlive();
			gameObject.SetActive(isAlive);
			if ( isAlive ) {
				Init();
			} else {
				DeInit();
			}
		}

		void Init() {
			DeInit();
			foreach ( var handler in _actions ) {
				Add(handler);
			}
		}

		void DeInit() {
			var items = _items.ToArray();
			foreach ( var item in items ) {
				Remove(item);
			}
		}

		void Add(IActionHandler handler) {
			var instance = GetOrCreateItem();
			instance.Init(handler, _playerEntity, _locationEntity);
			_items.Add(instance);
		}

		LocationActionView GetOrCreateItem() {
			if ( _itemPool.Count > 0 ) {
				return _itemPool.Pop();
			}
			return Instantiate(_globalData.LocationActionViewPrefab, _parent);
		}

		void Remove(LocationActionView itemView) {
			itemView.DeInit();
			_items.Remove(itemView);
			_itemPool.Push(itemView);
		}
	}
}