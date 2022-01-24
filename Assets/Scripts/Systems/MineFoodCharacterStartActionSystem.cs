using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;
using LocationService = EconomicsGame.Services.LocationService;

namespace EconomicsGame.Systems {
	public sealed class MineFoodCharacterStartActionSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, MineFoodCharacterActionEvent>.Exclude<BusyCharacterFlag, DeadCharacterFlag> _filter;

		LocationService _locationService;

		public void Init() {
			_locationService = _runtimeData.LocationService;
		}

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				ref var mineFoodActionEvent = ref _filter.Get2(characterIdx);
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var mineFoodAction = ref characterEntity.Get<MineFoodCharacterAction>();
				var targetLocationEntity = _locationService.GetEntity(mineFoodActionEvent.TargetLocation);
				ref var targetLocation = ref targetLocationEntity.Get<Location>();
				mineFoodAction.TargetLocation = targetLocation.Id;
				ref var actionProgress = ref characterEntity.Get<CharacterActionProgress>();
				actionProgress.Progress = 0;
				actionProgress.Speed = 0.2f;
				ref var source = ref targetLocationEntity.Get<FoodSource>();
				source.Locked++;
				characterEntity.Get<BusyCharacterFlag>();
				Debug.Log($"Character {characterEntity.Get<Character>().Log()} started to mine food at {targetLocation.Log()}");
			}
		}
	}
}