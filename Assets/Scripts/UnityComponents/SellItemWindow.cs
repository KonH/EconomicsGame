using EconomicsGame.Components;
using Leopotam.Ecs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace EconomicsGame.UnityComponents {
	public sealed class SellItemWindow : MonoBehaviour {
		[SerializeField] TMP_Text _headerText;
		[SerializeField] TMP_InputField _pricePerUnitInput;
		[SerializeField] Button _decPricePerUnitButton;
		[SerializeField] Button _incPricePerUnitButton;
		[SerializeField] TMP_InputField _countInput;
		[SerializeField] Button _decCountButton;
		[SerializeField] Button _incCountButton;
		[SerializeField] TMP_Text _totalPriceText;
		[SerializeField] Button _sellButton;
		[SerializeField] Button _cancelButton;

		readonly ReactiveProperty<double> _pricePerUnit = new ReactiveProperty<double>();
		readonly ReactiveProperty<double> _count = new ReactiveProperty<double>();
		readonly ReactiveProperty<double> _totalPrice = new ReactiveProperty<double>();

		CompositeDisposable _disposable;
		EcsEntity _itemEntity;

		public void Show(EcsEntity itemEntity) {
			_disposable?.Dispose();
			_disposable = new CompositeDisposable();
			_itemEntity = itemEntity;
			_headerText.text = $"Sell {itemEntity.Get<Item>().Name}";
			_pricePerUnit.Value = 0;
			_pricePerUnit
				.Subscribe(v => _pricePerUnitInput.text = v.ToString())
				.AddTo(_disposable);
			_pricePerUnit
				.Subscribe(v => _totalPrice.Value = v * _count.Value)
				.AddTo(_disposable);
			_pricePerUnit
				.Select(v => (v > 0))
				.SubscribeToInteractable(_decPricePerUnitButton)
				.AddTo(_disposable);
			_count.Value = 0;
			_count
				.Subscribe(v => _countInput.text = v.ToString())
				.AddTo(_disposable);
			_count
				.Subscribe(v => _totalPrice.Value = _pricePerUnit.Value * v)
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
				.SubscribeToInteractable(_sellButton)
				.AddTo(_disposable);
			gameObject.SetActive(true);
		}

		void Hide() {
			_disposable?.Dispose();
			gameObject.SetActive(false);
		}

		void Start() {
			_sellButton.onClick.AddListener(OnSellClick);
			_cancelButton.onClick.AddListener(OnCancelClick);
			_pricePerUnitInput.onValueChanged.AddListener(OnPricePerUnitChanged);
			_decPricePerUnitButton.onClick.AddListener(OnDecPriceClick);
			_incPricePerUnitButton.onClick.AddListener(OnIncPriceClick);
			_decCountButton.onClick.AddListener(OnDecCountClick);
			_incCountButton.onClick.AddListener(OnIncCountClick);
			_countInput.onValueChanged.AddListener(OnCountChanged);
			Hide();
		}

		void OnDestroy() => _disposable?.Dispose();

		void OnPricePerUnitChanged(string value) {
			_pricePerUnit.Value = double.TryParse(value, out var price) ? price : 0;
		}

		void OnCountChanged(string value) {
			_count.Value = double.TryParse(value, out var count) ? count : 0;
		}

		void OnDecPriceClick() => _pricePerUnit.Value--;

		void OnIncPriceClick() => _pricePerUnit.Value++;

		void OnDecCountClick() => _count.Value--;

		void OnIncCountClick() => _count.Value++;

		bool IsStateValid(double totalPrice) =>
			(totalPrice > 0) && (_count.Value <= _itemEntity.Get<Item>().Count.Value);

		void OnSellClick() {
			ref var sellEvent = ref _itemEntity.Get<SellItemEvent>();
			sellEvent.PricePerUnit = _pricePerUnit.Value;
			sellEvent.Count = _count.Value;
			Hide();
		}

		void OnCancelClick() => Hide();
	}
}