using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace EconomicsGame.Services {
	public class PersistantService {
		public string SaveRoot => Application.persistentDataPath;
		public string SavePath => Path.Combine(SaveRoot, "save.json");
		public bool CanLoad => File.Exists(SavePath);

		public void Save(List<List<object>> data) {
			var json = JsonConvert.SerializeObject(data, CreateSettings());
			File.WriteAllText(SavePath, json);
		}

		public List<List<object>> Load() {
			var json = File.ReadAllText(SavePath);
			var result = JsonConvert.DeserializeObject<List<List<object>>>(json, CreateSettings());
			return result;
		}

		public void Delete() => File.Delete(SavePath);

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