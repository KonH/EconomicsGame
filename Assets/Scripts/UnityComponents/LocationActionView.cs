using System;
using EconomicsGame.Services.ActionHandlers;
using Leopotam.Ecs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EconomicsGame.UnityComponents {
	public sealed class LocationActionView : MonoBehaviour {
		[SerializeField] TMP_Text _text;
		[SerializeField] Button _button;
		[SerializeField] Image _image;

		CompositeDisposable _disposable;
		EcsEntity _playerEntity;
		EcsEntity _locationEntity;
		IActionHandler _handler;

		public void Init(IActionHandler handler, EcsEntity playerEntity, EcsEntity locationEntity) {
			_disposable = new CompositeDisposable();
			gameObject.SetActive(true);
			_text.text = handler.Name;
			_handler = handler;
			_playerEntity = playerEntity;
			_locationEntity = locationEntity;
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
			var isVisible = IsVisible();
			gameObject.SetActive(isVisible);
			if ( !isVisible ) {
				return;
			}
			_button.interactable = IsInteractable();
			_image.fillAmount = GetProgress();
		}

		bool IsVisible() =>
			_handler?.IsPotentialPossible(_playerEntity, _locationEntity) ?? false;

		bool IsInteractable() =>
			_handler?.IsReallyPossible(_playerEntity) ?? false;

		float GetProgress() =>
			_handler?.GetProgress(_playerEntity) ?? 1.0f;

		void OnClick() {
			_handler?.Perform(_playerEntity, _locationEntity);
		}
	}
}