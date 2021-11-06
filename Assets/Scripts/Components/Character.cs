using UniRx;
using UnityEngine;

namespace EconomicsGame.Components {
	public struct Character : IPersistantComponent, IIdOwner {
		public int Id { get; set; }
		public string Name;
		public int CurrentLocation;
		public ReactiveProperty<Vector2> Position;
	}
}