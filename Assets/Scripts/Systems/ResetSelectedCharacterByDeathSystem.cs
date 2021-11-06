using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class ResetSelectedCharacterByDeathSystem : IEcsRunSystem {
		readonly EcsFilter<Character, SelectedCharacterFlag, CharacterDeathEvent> _filter;

		void IEcsRunSystem.Run() {
			foreach ( var characterIdx in _filter ) {
				ref var entity = ref _filter.GetEntity(characterIdx);
				entity.Del<SelectedCharacterFlag>();
			}
		}
	}
}