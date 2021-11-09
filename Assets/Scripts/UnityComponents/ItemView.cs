using EconomicsGame.Components;
using Leopotam.Ecs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EconomicsGame.UnityComponents {
	public sealed class ItemView : MonoBehaviour {
		SceneData _sceneData;
		EcsEntity _entity;
		string _name;
		CompositeDisposable _disposable;

		[SerializeField] TMP_Text _text;
		[SerializeField] Button _useButton;
		[SerializeField] Button _sellButton;

		public void Init(SceneData sceneData, EcsEntity entity, ref Item item, bool hasUseButton, bool canUse, bool canSell) {
			_sceneData = sceneData;
			_entity = entity;
			gameObject.SetActive(true);
			_useButton.gameObject.SetActive(hasUseButton);
			_sellButton.gameObject.SetActive(canSell);
			_name = item.Name;
			_disposable = new CompositeDisposable();
			item.Count
				.Subscribe(UpdateState)
				.AddTo(_disposable);
			_useButton.interactable = canUse;
		}

		public void DeInit() {
			gameObject.SetActive(false);
			_disposable?.Dispose();
		}

		void Start() {
			_useButton.onClick.AddListener(OnUseClick);
			_sellButton.onClick.AddListener(OnSellClick);
		}

		void UpdateState(double count) {
			_text.text = $"{_name} x{count:F}";
		}

		void OnDestroy() => _disposable?.Dispose();

		void OnUseClick() {
			_entity.Get<UseItemEvent>();
		}

		void OnSellClick() {
			_sceneData.SellItemWindow.Show(_entity);
		}
	}
}