using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using TMPro;
using UnityEngine;
using UniRx;

namespace EconomicsGame.UnityComponents {
	sealed class SelectedCharacterView : StartupInitializer {
		[SerializeField] TMP_Text _text;

		RuntimeData _runtimeData;

		public override void Attach(IEcsStartup startup) {
			_runtimeData = startup.RuntimeData;
		}

		void Start() {
			_runtimeData.SelectedCharacter.Subscribe(OnSelectedCharacterChanged);
		}

		void OnSelectedCharacterChanged(EcsEntity entity) {
			var isAlive = entity.IsAlive();
			gameObject.SetActive(isAlive);
			if ( isAlive && entity.Has<Character>() ) {
				Init(ref entity.Get<Character>());
			}
		}

		void Init(ref Character character) {
			_text.text = character.Name;
		}
	}
}