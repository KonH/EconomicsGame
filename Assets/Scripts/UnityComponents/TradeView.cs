using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EconomicsGame.UnityComponents {
	public sealed class TradeView : MonoBehaviour {
		RuntimeData _runtimeData;
		SceneData _sceneData;
		EcsEntity _buyer;
		EcsEntity _entity;
		string _name;
		long _price;
		CompositeDisposable _disposable;

		[SerializeField] TMP_Text _text;
		[SerializeField] Button _buyButton;

		public void Init(RuntimeData runtimeData, SceneData sceneData, EcsEntity buyer, EcsEntity entity, ref Item item, ref Trade trade, bool canBuy) {
			_runtimeData = runtimeData;
			_sceneData = sceneData;
			_buyer = buyer;
			_entity = entity;
			gameObject.SetActive(true);
			_buyButton.interactable = canBuy;
			_name = item.Name;
			_price = trade.PricePerUnit;
			_disposable = new CompositeDisposable();
			item.Count
				.Subscribe(UpdateState)
				.AddTo(_disposable);
		}

		public void DeInit() {
			gameObject.SetActive(false);
			_disposable?.Dispose();
		}

		void Start() {
			_buyButton.onClick.AddListener(OnBuyClick);
		}

		void UpdateState(long count) {
			_text.text = $"{_name} x{count} (${_price}/unit)";
		}

		void OnDestroy() => _disposable?.Dispose();

		void OnBuyClick() {
			_sceneData.BuyItemWindow.Show(_runtimeData, _buyer, _entity);
		}
	}
}