using System;
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
		Func<bool> _visibleChecker;
		Func<bool> _interactableChecker;
		Func<float> _progressHandler;
		Action _action;

		// TODO: pass action handler class?
		public void Init(string text, Func<bool> visibleChecker, Func<bool> interactableChecker, Func<float> progressHandler, Action action) {
			_disposable = new CompositeDisposable();
			gameObject.SetActive(true);
			_text.text = text;
			_visibleChecker = visibleChecker;
			_interactableChecker = interactableChecker;
			_progressHandler = progressHandler;
			_action = action;
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
			var isVisible = _visibleChecker?.Invoke() ?? false;
			gameObject.SetActive(isVisible);
			if ( !isVisible ) {
				return;
			}
			_button.interactable = _interactableChecker?.Invoke() ?? false;
			_image.fillAmount = _progressHandler?.Invoke() ?? 1.0f;
		}

		void OnClick() {
			_action?.Invoke();
		}
	}
}