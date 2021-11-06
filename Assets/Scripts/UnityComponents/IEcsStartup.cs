using EconomicsGame.Services;

namespace EconomicsGame.UnityComponents {
	public interface IEcsStartup {
		RuntimeData RuntimeData { get; }
		GlobalData GlobalData { get; }
		SceneData SceneData { get; }
	}
}