using System.Collections.Generic;
using UnityEngine;

namespace EconomicsGame.UnityComponents {
	public sealed class SceneData : MonoBehaviour {
		[SerializeField] StartupInitializer[] _initializers;
		[SerializeField] SellItemWindow _sellItemWindow;
		[SerializeField] BuyItemWindow _buyItemWindow;

		public IReadOnlyCollection<StartupInitializer> Initializers => _initializers;
		public SellItemWindow SellItemWindow => _sellItemWindow;
		public BuyItemWindow BuyItemWindow => _buyItemWindow;
	}
}