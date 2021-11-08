using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Systems {
	public sealed class RemoveCharacterByDeathSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, CharacterDeathEvent> _filter;

		public void Run() {
			var locationService = _runtimeData.LocationService;
			var characterService = _runtimeData.CharacterService;
			foreach ( var characterIdx in _filter ) {
				ref var character = ref _filter.Get1(characterIdx);
				ref var location = ref locationService.GetEntity(character.CurrentLocation).Get<Location>();
				characterService.RemoveCharacterFromLocation(character.Id, ref location);
			}
		}
	}
}