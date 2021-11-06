using UnityEngine;

namespace EconomicsGame.Components {
	public struct MoveCharacterAction : IPersistantComponent {
		public Vector2 SourcePosition;
		public Vector2 TargetPosition;
		public int TargetLocation;
	}
}