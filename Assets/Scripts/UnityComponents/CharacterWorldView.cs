using EconomicsGame.Components;
using UniRx;
using UnityEngine;

namespace EconomicsGame.UnityComponents {
	public sealed class CharacterWorldView : MonoBehaviour {
		CompositeDisposable _disposable;

		void OnDestroy() {
			_disposable?.Dispose();
		}

		public void Init(Character character) {
			_disposable = new CompositeDisposable();
			character.Position
				.Subscribe(OnPositionChanged)
				.AddTo(_disposable);
		}

		public void DeInit() {
			_disposable?.Dispose();
			gameObject.SetActive(false);
		}

		void OnPositionChanged(Vector2 position) {
			transform.localPosition = position;
		}
	}
}