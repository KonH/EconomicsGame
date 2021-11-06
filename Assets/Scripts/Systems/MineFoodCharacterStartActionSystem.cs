using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class MineFoodCharacterStartActionSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, MineFoodCharacterActionEvent>.Exclude<BusyCharacterFlag, DeadCharacterFlag> _filter;

		void IEcsRunSystem.Run() {
			var locationProvider = _runtimeData.LocationProvider;
			foreach ( var characterIdx in _filter ) {
				ref var mineFoodActionEvent = ref _filter.Get2(characterIdx);
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var mineFoodAction = ref characterEntity.Get<MineFoodCharacterAction>();
				Assert.IsTrue(locationProvider.TryGetEntity(mineFoodActionEvent.TargetLocation, out var targetLocationEntity));
				ref var targetLocation = ref targetLocationEntity.Get<Location>();
				mineFoodAction.TargetLocation = targetLocation.Id;
				ref var actionProgress = ref characterEntity.Get<CharacterActionProgress>();
				actionProgress.Progress = 0;
				actionProgress.Speed = 0.2f;
				ref var source = ref targetLocationEntity.Get<FoodSource>();
				source.Locked++;
				characterEntity.Get<BusyCharacterFlag>();
				Debug.Log($"Character {characterEntity.Get<Character>().Name} started to mine food");
			}
		}
	}
}