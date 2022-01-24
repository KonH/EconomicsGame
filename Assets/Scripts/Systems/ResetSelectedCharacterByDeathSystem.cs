using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class ResetSelectedCharacterByDeathSystem : IEcsRunSystem {
		readonly EcsFilter<Character, SelectedCharacterFlag, CharacterDeathEvent> _filter;

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				ref var entity = ref _filter.GetEntity(characterIdx);
				entity.Del<SelectedCharacterFlag>();
			}
		}
	}
}