using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using TMPro;
using UniRx;
using UnityEngine;

namespace EconomicsGame.UnityComponents {
	sealed class SelectedLocationView : StartupInitializer {
		[SerializeField] TMP_Text _text;

		RuntimeData _runtimeData;

		public override void Attach(IEcsStartup startup) {
			_runtimeData = startup.RuntimeData;
		}

		void Start() {
			_runtimeData.SelectedLocation.Subscribe(OnSelectedLocationChanged);
		}

		void OnSelectedLocationChanged(EcsEntity entity) {
			var isAlive = entity.IsAlive();
			gameObject.SetActive(isAlive);
			if ( isAlive && entity.Has<Location>() ) {
				Init(ref entity.Get<Location>());
			}
		}

		void Init(ref Location location) {
			_text.text = location.Name;
		}
	}
}