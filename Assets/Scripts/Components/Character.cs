using UniRx;
using UnityEngine;

namespace EconomicsGame.Components {
	public struct Character : IPersistantComponent {
		public int Id;
		public string Name;
		public int CurrentLocation;
		public ReactiveProperty<Vector2> Position;
	}
}