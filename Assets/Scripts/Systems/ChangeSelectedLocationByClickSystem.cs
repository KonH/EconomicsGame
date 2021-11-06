using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class ChangeSelectedLocationByClickSystem : IEcsRunSystem {
		readonly EcsFilter<Location, LocationClickEvent> _filter;

		void IEcsRunSystem.Run() {
			foreach ( var locationIdx in _filter ) {
				ref var entity = ref _filter.GetEntity(locationIdx);
				var isCurrent = entity.Has<SelectedLocationFlag>();
				if ( isCurrent ) {
					entity.Del<SelectedLocationFlag>();
				} else {
					entity.Get<SelectedLocationFlag>();
				}
			}
		}
	}
}