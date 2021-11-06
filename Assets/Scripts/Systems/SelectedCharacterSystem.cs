using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class SelectedCharacterSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, SelectedCharacterFlag>.Exclude<DeadCharacterFlag> _filter;

		public void Run() {
			EcsEntity character = default;
			foreach ( var characterIdx in _filter ) {
				character = _filter.GetEntity(characterIdx);
			}
			_runtimeData.SelectedCharacter.Value = character;
		}
	}
}