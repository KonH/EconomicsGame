namespace EconomicsGame.Services {
	public interface IStore {
		bool CanLoad { get; }
		void Save(string json);
		string Load();
		void Delete();
	}
}