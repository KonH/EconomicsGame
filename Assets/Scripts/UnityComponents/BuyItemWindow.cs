using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EconomicsGame.UnityComponents {
	public sealed class BuyItemWindow : MonoBehaviour {
		[SerializeField] TMP_Text _headerText;
		[SerializeField] TMP_Text _pricePerUnitText;
		[SerializeField] TMP_InputField _countInput;
		[SerializeField] Button _decCountButton;
		[SerializeField] Button _incCountButton;
		[SerializeField] TMP_Text _totalPriceText;
		[SerializeField] Button _buyButton;
		[SerializeField] Button _cancelButton;

		readonly ReactiveProperty<double> _count = new ReactiveProperty<double>();
		readonly ReactiveProperty<double> _totalPrice = new ReactiveProperty<double>();

		CompositeDisposable _disposable;
		RuntimeData _runtimeData;
		EcsEntity _characterEntity;
		EcsEntity _itemEntity;

		public void Show(RuntimeData runtimeData, EcsEntity characterEntity, EcsEntity itemEntity) {
			_disposable?.Dispose();
			_disposable = new CompositeDisposable();
			_runtimeData = runtimeData;
			_characterEntity = characterEntity;
			_itemEntity = itemEntity;
			_headerText.text = $"Sell {itemEntity.Get<Item>().Name}";
			_pricePerUnitText.text = $"Price: {itemEntity.Get<Trade>().PricePerUnit}";
			_count.Value = 0;
			_count
				.Subscribe(v => _countInput.text = v.ToString("F"))
				.AddTo(_disposable);
			var pricePerUnit = itemEntity.Get<Trade>().PricePerUnit;
			_count
				.Subscribe(v => _totalPrice.Value = pricePerUnit * v)
				.AddTo(_disposable);
			_count
				.Select(v => (v > 0))
				.SubscribeToInteractable(_decCountButton)
				.AddTo(_disposable);
			_count
				.Select(v => (v < itemEntity.Get<Item>().Count.Value))
				.SubscribeToInteractable(_incCountButton)
				.AddTo(_disposable);
			_totalPrice.Value = 0;
			_totalPrice
				.Subscribe(v => _totalPriceText.text = $"Total: {v}")
				.AddTo(_disposable);
			_totalPrice
				.Select(IsStateValid)
				.SubscribeToInteractable(_buyButton)
				.AddTo(_disposable);
			gameObject.SetActive(true);
		}

		void Hide() {
			_disposable?.Dispose();
			gameObject.SetActive(false);
		}

		void Start() {
			_buyButton.onClick.AddListener(OnBuyClick);
			_decCountButton.onClick.AddListener(OnDecCountClick);
			_incCountButton.onClick.AddListener(OnIncCountClick);
			_cancelButton.onClick.AddListener(OnCancelClick);
			_countInput.onValueChanged.AddListener(OnCountChanged);
			Hide();
		}

		void OnDestroy() => _disposable?.Dispose();

		void OnCountChanged(string value) {
			_count.Value = double.TryParse(value, out var count) ? count : 0;
		}

		void OnDecCountClick() => _count.Value--;

		void OnIncCountClick() => _count.Value++;

		bool IsStateValid(double totalPrice) {
			if ( (_count.Value <= 0) || (_count.Value > _itemEntity.Get<Item>().Count.Value) ) {
				return false;
			}
			var currentCash = _runtimeData.CashService.GetCurrentCash(ref _characterEntity);
			return currentCash >= totalPrice;
		}

		void OnBuyClick() {
			ref var buyer = ref _characterEntity.Get<Character>();
			ref var buyEvent = ref _itemEntity.Get<BuyItemEvent>();
			buyEvent.Buyer = buyer.Id;
			buyEvent.Count = _count.Value;
			Hide();
		}

		void OnCancelClick() => Hide();
	}
}