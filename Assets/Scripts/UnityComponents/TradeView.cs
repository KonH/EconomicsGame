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
		double _price;
		bool _canBuyBase;
		CompositeDisposable _disposable;

		[SerializeField] TMP_Text _text;
		[SerializeField] Button _buyButton;

		readonly ReactiveProperty<bool> _canBuy = new ReactiveProperty<bool>();

		public void Init(RuntimeData runtimeData, SceneData sceneData, EcsEntity buyer, EcsEntity entity, ref Item item, ref Trade trade, bool canBuyBase) {
			_runtimeData = runtimeData;
			_sceneData = sceneData;
			_buyer = buyer;
			_entity = entity;
			gameObject.SetActive(true);
			_name = item.Name;
			_price = trade.PricePerUnit;
			_canBuyBase = canBuyBase;
			_disposable = new CompositeDisposable();
			item.Count
				.Subscribe(UpdateState)
				.AddTo(_disposable);
			_canBuy
				.SubscribeToInteractable(_buyButton)
				.AddTo(_disposable);
		}

		public void DeInit() {
			gameObject.SetActive(false);
			_disposable?.Dispose();
		}

		void Start() {
			_buyButton.onClick.AddListener(OnBuyClick);
		}

		void Update() {
			_canBuy.Value = CanBuy();
		}

		bool CanBuy() {
			if ( !_canBuyBase ) {
				return false;
			}
			var currentCash = _runtimeData.CashService.GetCurrentCash(ref _buyer);
			return currentCash >= _price;
		}

		void UpdateState(double count) {
			_text.text = $"{_name} x{count:F} (${_price:F}/unit)";
		}

		void OnDestroy() => _disposable?.Dispose();

		void OnBuyClick() {
			_sceneData.BuyItemWindow.Show(_runtimeData, _buyer, _entity);
		}
	}
}