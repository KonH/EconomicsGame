using UniRx;
using UnityEngine;

namespace EconomicsGame.Components {
	public struct Location : IPersistantComponent {
		public int Id;
		public string Name;
		public Vector2 Position;
		public ReactiveCollection<int> Characters;
	}
}