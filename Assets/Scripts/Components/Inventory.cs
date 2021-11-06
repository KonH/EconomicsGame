using UniRx;

namespace EconomicsGame.Components {
	public struct Inventory : IPersistantComponent {
		public ReactiveCollection<int> Items;
	}
}