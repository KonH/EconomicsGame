using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace EconomicsGame.UnityComponents {
	public sealed class StatView : MonoBehaviour {
		CompositeDisposable _disposable;
		string _key;

		[SerializeField] TMP_Text _text;

		public void Init(string key, ReactiveProperty<float> value) {
			gameObject.SetActive(true);
			_key = key;
			_disposable = new CompositeDisposable();
			value
				.Subscribe(UpdateState)
				.AddTo(_disposable);
		}

		public void DeInit() {
			gameObject.SetActive(false);
			_disposable?.Dispose();
		}

		void UpdateState(float value) {
			_text.text = $"{_key} {FormatValue(value)}";
		}

		string FormatValue(float value) =>
			Math.Round(value, 2).ToString("0.00");

		void OnDestroy() => _disposable?.Dispose();
	}
}