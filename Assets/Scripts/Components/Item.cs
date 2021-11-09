using UniRx;

namespace EconomicsGame.Components {
	public struct Item : IPersistantComponent {
		public int Id;
		public int Owner;
		public string Name;
		public ReactiveProperty<double> Count;
	}
}