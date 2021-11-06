using System;
using System.Collections.Generic;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using UniRx;

namespace EconomicsGame.UnityComponents {
	// TODO: can not use for non-player characters
	// TODO: separated classes for action handlers / rework can checks to re-use it in bots
	sealed class LocationActionsView : StartupInitializer {
		readonly List<(
			string,
			Func<EcsEntity, EcsEntity, bool>,
			Func<EcsEntity, EcsEntity, bool>,
			Action<EcsEntity, EcsEntity>
			)>
			_actions = new List<(
				string,
				Func<EcsEntity, EcsEntity, bool>,
				Func<EcsEntity, EcsEntity, bool>,
				Action<EcsEntity, EcsEntity>)> {
			("Move", ShowMove, CanMove, Move),
			("Mine Food", ShowMineFood, CanMineFood, MineFood)
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
			foreach ( var pair in _actions ) {
				var (text, visibleChecker, interactableChecker, action) = pair;
				Add(text,
					() => visibleChecker(_playerEntity, _locationEntity),
					() => interactableChecker(_playerEntity, _locationEntity),
					() => GetProgress(_playerEntity),
					() => action(_playerEntity, _locationEntity));
			}
		}

		void DeInit() {
			var items = _items.ToArray();
			foreach ( var item in items ) {
				Remove(item);
			}
		}

		void Add(string text, Func<bool> visibleChecker, Func<bool> interactableChecker, Func<float> progressHandler, Action action) {
			var instance = GetOrCreateItem();
			instance.Init(text, visibleChecker, interactableChecker, progressHandler, action);
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

		static float GetProgress(EcsEntity characterEntity) {
			if ( characterEntity.Has<CharacterActionProgress>() ) {
				ref var actionProgress = ref characterEntity.Get<CharacterActionProgress>();
				return actionProgress.Progress;
			}
			return 1.0f;
		}

		static bool ShowMove(EcsEntity characterEntity, EcsEntity locationEntity) {
			return characterEntity.Get<Character>().CurrentLocation != locationEntity.Get<Location>().Id;
		}

		static bool CanMove(EcsEntity characterEntity, EcsEntity locationEntity) {
			return !characterEntity.Has<BusyCharacterFlag>();
		}

		static void Move(EcsEntity characterEntity, EcsEntity locationEntity) {
			ref var moveAction = ref characterEntity.Get<MoveCharacterActionEvent>();
			moveAction.TargetLocation = locationEntity.Get<Location>().Id;
		}

		static bool ShowMineFood(EcsEntity characterEntity, EcsEntity locationEntity) {
			if ( characterEntity.Get<Character>().CurrentLocation != locationEntity.Get<Location>().Id ) {
				return false;
			}
			if ( !locationEntity.Has<FoodSource>() ) {
				return false;
			}
			ref var source = ref locationEntity.Get<FoodSource>();
			return (source.Remaining > 0) && (source.Locked < source.Remaining);
		}

		static bool CanMineFood(EcsEntity characterEntity, EcsEntity locationEntity) {
			return !characterEntity.Has<BusyCharacterFlag>();
		}

		static void MineFood(EcsEntity characterEntity, EcsEntity locationEntity) {
			ref var mineFoodAction = ref characterEntity.Get<MineFoodCharacterActionEvent>();
			mineFoodAction.TargetLocation = locationEntity.Get<Location>().Id;
		}
	}
}