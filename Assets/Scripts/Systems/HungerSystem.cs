using EconomicsGame.Components;
using Leopotam.Ecs;
using UnityEngine;

namespace EconomicsGame.Systems {
	public sealed class HungerSystem : IEcsRunSystem {
		EcsFilter<Character, CharacterStats>.Exclude<DeadCharacterFlag> _filter;

		public void Run() {
			foreach ( var characterIdx in _filter ) {
				ref var stats = ref _filter.Get2(characterIdx);
				if ( !stats.Values.TryGetValue("Hunger", out var hunger) ) {
					continue;
				}
				hunger.Value = Mathf.Clamp01(hunger.Value + 0.025f * Time.deltaTime);
				if ( (hunger.Value < 1) || !stats.Values.TryGetValue("Health", out var health) ) {
					continue;
				}
				health.Value = Mathf.Clamp01(health.Value - 0.01f * Time.deltaTime);
			}
		}
	}
}