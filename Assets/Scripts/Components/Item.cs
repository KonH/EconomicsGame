using UniRx;

namespace EconomicsGame.Components {
	public struct Item : IPersistantComponent, IIdOwner {
		public int Id { get; set; }
		public int Owner;
		public string Name;
		public ReactiveProperty<double> Count;
	}
}