using System.Collections.Generic;

namespace EconomicsGame.Services {
	public sealed class PersistantService {
		readonly JsonSerializerWrapper _serializer = new JsonSerializerWrapper();
		readonly IStore _store;

		public bool CanLoad => _store.CanLoad;

		public PersistantService(IStore store) {
			_store = store;
		}

		public void Save(List<List<object>> data) {
			var json = _serializer.Serialize(data);
			_store.Save(json);
		}

		public List<List<object>> Load() {
			var json = _store.Load();
			var result = _serializer.Deserialize<List<List<object>>>(json);
			return result;
		}

		public void Delete() => _store.Delete();
	}
}