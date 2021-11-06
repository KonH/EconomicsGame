using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class BotMineFoodSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		public void Run() {
			var locationProvider = _runtimeData.LocationProvider;
			foreach ( var characterIdx in _filter ) {
				ref var character = ref _filter.Get1(characterIdx);
				Assert.IsTrue(locationProvider.TryGetEntity(character.CurrentLocation, out var currentLocationEntity));
				if ( !currentLocationEntity.Has<FoodSource>() ) {
					continue;
				}
				ref var currentFoodSource = ref currentLocationEntity.Get<FoodSource>();
				var isEnoughFood = (currentFoodSource.Remaining > 0) && (currentFoodSource.Locked < currentFoodSource.Remaining);
				if ( !isEnoughFood ) {
					continue;
				}
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var mineFoodEvent = ref characterEntity.Get<MineFoodCharacterActionEvent>();
				mineFoodEvent.TargetLocation = character.CurrentLocation;
			}
		}
	}
}