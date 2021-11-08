using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class MineFoodCharacterStartActionSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, MineFoodCharacterActionEvent>.Exclude<BusyCharacterFlag, DeadCharacterFlag> _filter;

		void IEcsRunSystem.Run() {
			var locationService = _runtimeData.LocationService;
			foreach ( var characterIdx in _filter ) {
				ref var mineFoodActionEvent = ref _filter.Get2(characterIdx);
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var mineFoodAction = ref characterEntity.Get<MineFoodCharacterAction>();
				var targetLocationEntity = locationService.GetEntity(mineFoodActionEvent.TargetLocation);
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