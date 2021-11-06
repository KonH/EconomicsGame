using UniRx;

namespace EconomicsGame.Components {
	public struct CharacterStats : IPersistantComponent {
		public ReactiveDictionary<string, ReactiveProperty<float>> Values;
	}
}