using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class WorldInitSystem : IEcsInitSystem {
		readonly EcsWorld _world;
		readonly RuntimeData _runtimeData;

		public void Init() {
			var loader = new WorldLoader(_world, _runtimeData);
			if ( !loader.TryLoad() ) {
				var generator = new WorldGenerator(_runtimeData);
				generator.Generate();
			}
		}
	}
}