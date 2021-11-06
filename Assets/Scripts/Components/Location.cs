using UniRx;
using UnityEngine;

namespace EconomicsGame.Components {
	public struct Location : IPersistantComponent, IIdOwner {
		public int Id { get; set; }
		public string Name;
		public Vector2 Position;
		public ReactiveCollection<int> Characters;
		public ReactiveCollection<int> Trades;
	}
}