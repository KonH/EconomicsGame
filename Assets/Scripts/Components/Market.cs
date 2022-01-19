using UniRx;

namespace EconomicsGame.Components {
	public struct Market : IPersistantComponent {
		public ReactiveCollection<int> Trades;
	}
}