using System.Collections.Generic;
using System.Linq;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.UnityComponents {
	sealed class LocationCharactersView : StartupInitializer {
		readonly Dictionary<int, LocationCharacterView> _items = new Dictionary<int, LocationCharacterView>();
		readonly Stack<LocationCharacterView> _itemPool = new Stack<LocationCharacterView>();

		RuntimeData _runtimeData;
		GlobalData _globalData;
		CompositeDisposable _disposable;

		[SerializeField] Transform _parent;

		public override void Attach(IEcsStartup startup) {
			_runtimeData = startup.RuntimeData;
			_globalData = startup.GlobalData;
		}

		void Start() {
			_runtimeData.SelectedLocation.Subscribe(OnSelectedLocationChanged);
		}

		void OnSelectedLocationChanged(EcsEntity entity) {
			var isAlive = entity.IsAlive();
			gameObject.SetActive(isAlive);
			if ( isAlive && entity.Has<Location>() ) {
				Init(ref entity.Get<Location>());
			} else {
				DeInit();
			}
		}

		void Init(ref Location location) {
			DeInit();
			_disposable = new CompositeDisposable();
			foreach ( var item in location.Characters ) {
				OnAdd(ConvertIdToEntity(item));
			}
			location.Characters
				.ObserveAdd()
				.Select(e => ConvertIdToEntity(e.Value))
				.Subscribe(OnAdd)
				.AddTo(_disposable);
			location.Characters
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
			_runtimeData.CharacterService.GetEntity(id);

		void OnAdd(EcsEntity entity) {
			ref var character = ref entity.Get<Character>();
			var instance = GetOrCreateItem();
			instance.Init(entity, ref character);
			_items[character.Id] = instance;
		}

		LocationCharacterView GetOrCreateItem() {
			if ( _itemPool.Count > 0 ) {
				return _itemPool.Pop();
			}
			return Instantiate(_globalData.LocationCharacterViewPrefab, _parent);
		}

		void OnRemove(int id) {
			if ( _items.TryGetValue(id, out var item) ) {
				Remove(id, item);
			}
		}

		void Remove(int id, LocationCharacterView locationCharacterView) {
			locationCharacterView.DeInit();
			_items.Remove(id);
			_itemPool.Push(locationCharacterView);
		}
	}
}