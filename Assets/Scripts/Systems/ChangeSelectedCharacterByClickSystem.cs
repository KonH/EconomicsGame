using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class ChangeSelectedCharacterByClickSystem : IEcsRunSystem {
		readonly EcsFilter<Character, CharacterClickEvent>.Exclude<DeadCharacterFlag> _filter;

		void IEcsRunSystem.Run() {
			foreach ( var characterIdx in _filter ) {
				ref var entity = ref _filter.GetEntity(characterIdx);
				var isCurrent = entity.Has<SelectedCharacterFlag>();
				if ( isCurrent ) {
					entity.Del<SelectedCharacterFlag>();
				} else {
					entity.Get<SelectedCharacterFlag>();
				}
			}
		}
	}
}