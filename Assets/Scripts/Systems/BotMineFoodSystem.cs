using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class BotMineFoodSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, BotCharacterFlag, Inventory>.Exclude<DeadCharacterFlag, BusyCharacterFlag> _filter;

		LocationService _locationService;

		public void Init() {
			_locationService = _runtimeData.LocationService;
		}

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				ref var character = ref _filter.Get1(characterIdx);
				var currentLocationEntity = _locationService.GetEntity(character.CurrentLocation);
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