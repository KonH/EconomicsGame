using UnityEngine;

namespace EconomicsGame.UnityComponents {
	[CreateAssetMenu]
	public sealed class GlobalData : ScriptableObject {
		[SerializeField] LocationWorldView _locationWorldViewPrefab;
		[SerializeField] CharacterWorldView _characterWorldViewPrefab;
		[SerializeField] ItemView _itemViewPrefab;
		[SerializeField] TradeView _tradeViewPrefab;
		[SerializeField] StatView _statViewPrefab;
		[SerializeField] LocationCharacterView _locationCharacterViewPrefab;
		[SerializeField] LocationActionView _locationActionViewPrefab;

		public LocationWorldView LocationWorldViewPrefab => _locationWorldViewPrefab;
		public CharacterWorldView CharacterWorldViewPrefab => _characterWorldViewPrefab;
		public ItemView ItemViewPrefab => _itemViewPrefab;
		public TradeView TradeViewPrefab => _tradeViewPrefab;
		public StatView StatViewPrefab => _statViewPrefab;
		public LocationCharacterView LocationCharacterViewPrefab => _locationCharacterViewPrefab;
		public LocationActionView LocationActionViewPrefab => _locationActionViewPrefab;
	}
}