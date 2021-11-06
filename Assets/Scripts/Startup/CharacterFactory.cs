using System.Collections.Generic;
using EconomicsGame.Components;
using EconomicsGame.UnityComponents;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Startup {
	class CharacterFactory : StartupInitializer {
		readonly Dictionary<int, CharacterWorldView> _items = new Dictionary<int, CharacterWorldView>();

		CompositeDisposable _disposable;
		CharacterWorldView _prefab;

		public override void Attach(IEcsStartup startup) {
			_disposable = new CompositeDisposable();
			_prefab = startup.GlobalData.CharacterWorldViewPrefab;
			var characters = startup.RuntimeData.Characters;
			characters
				.ObserveAdd()
				.Subscribe(e => OnAdd(e.Value))
				.AddTo(_disposable);
			characters
				.ObserveRemove()
				.Subscribe(e => OnRemove(e.Value))
				.AddTo(_disposable);
		}

		void OnDestroy() {
			_disposable?.Dispose();
		}

		void OnAdd(EcsEntity entity) {
			ref var character = ref entity.Get<Character>();
			var instance = Instantiate(_prefab, character.Position.Value, Quaternion.identity);
			instance.Init(character);
			_items[character.Id] = instance;
		}

		void OnRemove(EcsEntity entity) {
			ref var character = ref entity.Get<Character>();
			_items[character.Id].DeInit();
			_items.Remove(character.Id);
		}
	}
}