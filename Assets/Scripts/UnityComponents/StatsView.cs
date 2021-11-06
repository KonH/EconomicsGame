using System.Collections.Generic;
using System.Linq;
using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using UniRx;

namespace EconomicsGame.UnityComponents {
	sealed class StatsView : StartupInitializer {
		readonly Dictionary<string, StatView> _stats = new Dictionary<string, StatView>();
		readonly Stack<StatView> _statPool = new Stack<StatView>();

		RuntimeData _runtimeData;
		GlobalData _globalData;
		CompositeDisposable _disposable;

		[SerializeField] Transform _parent;

		public override void Attach(IEcsStartup startup) {
			_runtimeData = startup.RuntimeData;
			_globalData = startup.GlobalData;
		}

		void Start() {
			_runtimeData.SelectedCharacter.Subscribe(OnSelectedCharacterChanged);
		}

		void OnSelectedCharacterChanged(EcsEntity entity) {
			var isAlive = entity.IsAlive();
			gameObject.SetActive(isAlive);
			if ( isAlive && entity.Has<CharacterStats>() ) {
				Init(ref entity.Get<CharacterStats>());
			} else {
				DeInit();
			}
		}

		void Init(ref CharacterStats stats) {
			DeInit();
			_disposable = new CompositeDisposable();
			var index = 0;
			foreach ( var pair in stats.Values ) {
				var statView = OnAdd(pair.Key, pair.Value);
				statView.transform.SetSiblingIndex(index);
				index++;
			}
			stats.Values
				.ObserveAdd()
				.Subscribe(e => OnAdd(e.Key, e.Value))
				.AddTo(_disposable);
			stats.Values
				.ObserveRemove()
				.Subscribe(e => OnRemove(e.Key))
				.AddTo(_disposable);
		}

		void DeInit() {
			_disposable?.Dispose();
			var keys = _stats.Keys.ToArray();
			foreach ( var key in keys ) {
				Remove(key, _stats[key]);
			}
		}

		StatView OnAdd(string key, ReactiveProperty<float> value) {
			var instance = GetOrCreateStat();
			instance.Init(key, value);
			_stats[key] = instance;
			return instance;
		}

		StatView GetOrCreateStat() {
			if ( _statPool.Count > 0 ) {
				return _statPool.Pop();
			}
			return Instantiate(_globalData.StatViewPrefab, _parent);
		}

		void OnRemove(string key) {
			if ( _stats.TryGetValue(key, out var stat) ) {
				Remove(key, stat);
			}
		}

		void Remove(string key, StatView statView) {
			statView.DeInit();
			_stats.Remove(key);
			_statPool.Push(statView);
		}
	}
}