using EconomicsGame.Components;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.UnityComponents {
	public sealed class ControlsHandler : StartupInitializer {
		EcsWorld _world;

		public override void Attach(IEcsStartup startup) {
			_world = startup.RuntimeData.World;
		}

		void Update() {
			if ( Input.GetKeyDown(KeyCode.S) ) {
				_world.NewEntity().Get<SaveStateEvent>();
			}
		}
	}
}