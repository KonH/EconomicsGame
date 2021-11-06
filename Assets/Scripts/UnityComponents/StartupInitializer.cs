using UnityEngine;

namespace EconomicsGame.UnityComponents {
	public abstract class StartupInitializer : MonoBehaviour {
		public abstract void Attach(IEcsStartup startup);
	}
}