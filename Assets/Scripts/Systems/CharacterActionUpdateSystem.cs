using EconomicsGame.Components;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class CharacterActionUpdateSystem : IEcsRunSystem {
		readonly EcsFilter<Character, CharacterActionProgress>.Exclude<DeadCharacterFlag> _filter;

		void IEcsRunSystem.Run() {
			foreach ( var characterIdx in _filter ) {
				ref var actionProgress = ref _filter.Get2(characterIdx);
				actionProgress.Progress += Time.deltaTime * actionProgress.Speed;
			}
		}
	}
}