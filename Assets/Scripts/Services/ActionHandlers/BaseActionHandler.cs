using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Services.ActionHandlers {
	public abstract class BaseActionHandler : IActionHandler {
		public abstract string Name { get; }

		public abstract bool IsPotentialPossible(EcsEntity characterEntity, EcsEntity locationEntity);

		public bool IsReallyPossible(EcsEntity characterEntity) =>
			!characterEntity.Has<BusyCharacterFlag>();

		public abstract void Perform(EcsEntity characterEntity, EcsEntity locationEntity);

		public float GetProgress(EcsEntity characterEntity) {
			if ( characterEntity.Has<CharacterActionProgress>() ) {
				ref var actionProgress = ref characterEntity.Get<CharacterActionProgress>();
				return actionProgress.Progress;
			}
			return 1.0f;
		}
	}
}