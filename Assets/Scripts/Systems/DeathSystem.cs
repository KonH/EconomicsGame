using EconomicsGame.Components;
using EconomicsGame.Services;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class DeathSystem : IEcsRunSystem {
		readonly EcsFilter<Character, CharacterStats>.Exclude<DeadCharacterFlag> _filter;

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				ref var character = ref _filter.Get1(characterIdx);
				ref var stats = ref _filter.Get2(characterIdx);
				if ( !stats.Values.TryGetValue("Health", out var health) || (health.Value > 0) ) {
					continue;
				}
				ref var entity = ref _filter.GetEntity(characterIdx);
				entity.Get<DeadCharacterFlag>();
				entity.Get<CharacterDeathEvent>();
				Debug.Log($"Character {character.Log()} died");
			}
		}
	}
}