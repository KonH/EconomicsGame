using EconomicsGame.Components;
using Leopotam.Ecs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EconomicsGame.UnityComponents {
	public sealed class LocationCharacterView : MonoBehaviour {
		CompositeDisposable _disposable;
		EcsEntity _entity;

		[SerializeField] TMP_Text _text;
		[SerializeField] Button _button;

		public void Init(EcsEntity entity, ref Character character) {
			_disposable = new CompositeDisposable();
			_entity = entity;
			gameObject.SetActive(true);
			_text.text = $"{character.Name}";
			Observable.EveryUpdate()
				.Subscribe(_ => UpdateState())
				.AddTo(_disposable);
		}

		public void DeInit() {
			_disposable?.Dispose();
			gameObject.SetActive(false);
		}

		void Start() {
			_button.onClick.AddListener(OnClick);
		}

		void OnDestroy() {
			_disposable?.Dispose();
		}

		void UpdateState() {
			_button.interactable = _entity.IsAlive() && !_entity.Has<SelectedCharacterFlag>();
		}

		void OnClick() {
			_entity.Get<CharacterClickEvent>();
		}
	}
}