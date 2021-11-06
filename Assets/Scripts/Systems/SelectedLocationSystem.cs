using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class SelectedLocationSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Location, SelectedLocationFlag> _filter;

		public void Run() {
			EcsEntity location = default;
			foreach ( var locationIdx in _filter ) {
				location = _filter.GetEntity(locationIdx);
			}
			_runtimeData.SelectedLocation.Value = location;
		}
	}
}