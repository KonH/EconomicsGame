using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Services.ActionHandlers {
	public interface IActionHandler {
		string Name { get; }
		bool IsPotentialPossible(EcsEntity characterEntity, EcsEntity locationEntity);
		bool IsReallyPossible(EcsEntity characterEntity);
		void Perform(EcsEntity characterEntity, EcsEntity locationEntity);
		float GetProgress(EcsEntity characterEntity);
	}
}