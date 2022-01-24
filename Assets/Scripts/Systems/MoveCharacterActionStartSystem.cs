using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using LocationService = EconomicsGame.Services.LocationService;

namespace EconomicsGame.Systems {
	public sealed class MoveCharacterActionStartSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, MoveCharacterActionEvent>.Exclude<BusyCharacterFlag, DeadCharacterFlag> _filter;

		LocationService _locationService;

		public void Init() {
			_locationService = _runtimeData.LocationService;
		}

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				ref var moveActionEvent = ref _filter.Get2(characterIdx);
				ref var character = ref _filter.Get1(characterIdx);
				if ( character.CurrentLocation == moveActionEvent.TargetLocation ) {
					continue;
				}
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var moveAction = ref characterEntity.Get<MoveCharacterAction>();
				moveAction.SourcePosition = character.Position.Value;
				ref var currentLocation = ref _locationService.GetEntity(character.CurrentLocation).Get<Location>();
				ref var targetLocation = ref _locationService.GetEntity(moveActionEvent.TargetLocation).Get<Location>();
				moveAction.TargetPosition = targetLocation.Position;
				moveAction.TargetLocation = targetLocation.Id;
				ref var actionProgress = ref characterEntity.Get<CharacterActionProgress>();
				actionProgress.Progress = 0;
				actionProgress.Speed = 1;
				currentLocation.Characters.Remove(character.Id);
				characterEntity.Get<BusyCharacterFlag>();
				Debug.Log($"Character {character.Log()} started to move to {targetLocation.Log()}");
			}
		}
	}
}