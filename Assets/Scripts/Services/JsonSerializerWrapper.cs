using Newtonsoft.Json;

namespace EconomicsGame.Services {
	public sealed class JsonSerializerWrapper {
		public string Serialize(object data) =>
			JsonConvert.SerializeObject(data, CreateSettings());

		public T Deserialize<T>(string json) =>
			JsonConvert.DeserializeObject<T>(json, CreateSettings());

		JsonSerializerSettings CreateSettings() {
			var settings = new JsonSerializerSettings {
				TypeNameHandling = TypeNameHandling.Auto,
				Formatting = Formatting.Indented
			};
			settings.Converters.Add(new Vector2JsonConverter());
			settings.Converters.Add(new ReactivePropertyConverter());
			return settings;
		}
	}
}