using EconomicsGame.Components;
using EconomicsGame.UnityComponents;
using Leopotam.Ecs;
using UniRx;
using UnityEngine;

namespace EconomicsGame.Startup {
	class LocationFactory : StartupInitializer {
		CompositeDisposable _disposable;
		LocationWorldView _prefab;

		public override void Attach(IEcsStartup startup) {
			_disposable = new CompositeDisposable();
			_prefab = startup.GlobalData.LocationWorldViewPrefab;
			var locations = startup.RuntimeData.LocationService.Locations;
			locations
				.ObserveAdd()
				.Subscribe(e => OnAdd(e.Value))
				.AddTo(_disposable);
		}

		void OnDestroy() {
			_disposable?.Dispose();
		}

		void OnAdd(EcsEntity entity) {
			ref var location = ref entity.Get<Location>();
			var instance = Instantiate(_prefab, location.Position, Quaternion.identity);
			instance.Init(entity);
		}
	}
}