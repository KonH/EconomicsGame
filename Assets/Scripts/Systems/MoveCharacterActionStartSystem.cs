using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class MoveCharacterActionStartSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, MoveCharacterActionEvent>.Exclude<BusyCharacterFlag, DeadCharacterFlag> _filter;

		void IEcsRunSystem.Run() {
			var locationProvider = _runtimeData.LocationProvider;
			foreach ( var characterIdx in _filter ) {
				ref var moveActionEvent = ref _filter.Get2(characterIdx);
				ref var character = ref _filter.Get1(characterIdx);
				if ( character.CurrentLocation == moveActionEvent.TargetLocation ) {
					continue;
				}
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var moveAction = ref characterEntity.Get<MoveCharacterAction>();
				moveAction.SourcePosition = character.Position.Value;
				Assert.IsTrue(locationProvider.TryGetComponent(character.CurrentLocation, out var currentLocation));
				Assert.IsTrue(locationProvider.TryGetComponent(moveActionEvent.TargetLocation, out var targetLocation));
				moveAction.TargetPosition = targetLocation.Position;
				moveAction.TargetLocation = targetLocation.Id;
				ref var actionProgress = ref characterEntity.Get<CharacterActionProgress>();
				actionProgress.Progress = 0;
				actionProgress.Speed = 1;
				currentLocation.Characters.Remove(character.Id);
				characterEntity.Get<BusyCharacterFlag>();
				Debug.Log($"Character {character.Name} started to move to location {targetLocation.Name}");
			}
		}
	}
}