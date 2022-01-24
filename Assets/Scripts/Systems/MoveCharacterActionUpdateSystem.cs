using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using LocationService = EconomicsGame.Services.LocationService;

namespace EconomicsGame.Systems {
	public sealed class MoveCharacterActionUpdateSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, MoveCharacterAction, CharacterActionProgress>.Exclude<DeadCharacterFlag> _filter;

		LocationService _locationService;
		CharacterService _characterService;

		public void Init() {
			_locationService = _runtimeData.LocationService;
			_characterService = _runtimeData.CharacterService;
		}

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var character = ref _filter.Get1(characterIdx);
				ref var moveAction = ref _filter.Get2(characterIdx);
				ref var actionProgress = ref _filter.Get3(characterIdx);
				character.Position.Value = Vector2.Lerp(moveAction.SourcePosition, moveAction.TargetPosition, actionProgress.Progress);
				var isCompleted = actionProgress.Progress >= 1;
				if ( !isCompleted ) {
					continue;
				}
				character.Position.Value = moveAction.TargetPosition;
				ref var location = ref _locationService.GetEntity(moveAction.TargetLocation).Get<Location>();
				_characterService.MoveCharacterToLocation(ref character, ref location);
				characterEntity.Del<MoveCharacterAction>();
				characterEntity.Del<BusyCharacterFlag>();
				Debug.Log($"Character {character.Log()} moved to {location.Log()}");
			}
		}
	}
}