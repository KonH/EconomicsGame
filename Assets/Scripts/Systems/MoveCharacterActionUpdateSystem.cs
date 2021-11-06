using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class MoveCharacterActionUpdateSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, MoveCharacterAction, CharacterActionProgress>.Exclude<DeadCharacterFlag> _filter;

		void IEcsRunSystem.Run() {
			var locationProvider = _runtimeData.LocationProvider;
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
				character.CurrentLocation = moveAction.TargetLocation;
				characterEntity.Del<MoveCharacterAction>();
				characterEntity.Del<BusyCharacterFlag>();
				Assert.IsTrue(locationProvider.TryGetComponent(character.CurrentLocation, out var location));
				location.Characters.Add(character.Id);
				Debug.Log($"Character {character.Name} moved to location {location.Name}");
			}
		}
	}
}