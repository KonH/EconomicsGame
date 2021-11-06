using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine.Assertions;

namespace EconomicsGame.Systems {
	public sealed class RemoveCharacterByDeathSystem : IEcsRunSystem {
		readonly RuntimeData _runtimeData;
		readonly EcsFilter<Character, CharacterDeathEvent> _filter;

		public void Run() {
			var characterProvider = _runtimeData.CharacterProvider;
			var locationProvider = _runtimeData.LocationProvider;
			foreach ( var characterIdx in _filter ) {
				ref var characterEntity = ref _filter.GetEntity(characterIdx);
				ref var character = ref _filter.Get1(characterIdx);
				Assert.IsTrue(locationProvider.TryGetComponent(character.CurrentLocation, out var location));
				location.Characters.Remove(character.Id);
				_runtimeData.Characters.Remove(characterEntity);
				characterProvider.Remove(character.Id);
			}
		}
	}
}