using System.IO;
using UnityEngine;

namespace EconomicsGame.Services {
	public sealed class PersistentDataFileStore : IStore {
		public string SaveRoot => Application.persistentDataPath;
		public string SavePath => Path.Combine(SaveRoot, "save.json");

		public bool CanLoad => File.Exists(SavePath);

		public void Save(string json) => File.WriteAllText(SavePath, json);

		public string Load() => File.ReadAllText(SavePath);

		public void Delete() => File.Delete(SavePath);
	}
}