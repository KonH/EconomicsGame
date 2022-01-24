using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class RemoveCharacterByDeathSystem : IEcsInitSystem, IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, CharacterDeathEvent> _filter;

		LocationService _locationService;
		CharacterService _characterService;

		public void Init() {
			_locationService = _runtimeData.LocationService;
			_characterService = _runtimeData.CharacterService;
		}

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				ref var character = ref _filter.Get1(characterIdx);
				ref var location = ref _locationService.GetEntity(character.CurrentLocation).Get<Location>();
				_characterService.RemoveFromLocation(character.Id, ref location);
			}
		}
	}
}