namespace EconomicsGame.Services {
	public sealed class InMemoryStore : IStore {
		string _json;

		public bool CanLoad => (_json != null);

		public void Save(string json) {
			_json = json;
		}

		public string Load() => _json;

		public void Delete() {
			_json = null;
		}
	}
}