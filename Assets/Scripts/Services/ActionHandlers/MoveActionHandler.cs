using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Services.ActionHandlers {
	public sealed class MoveActionHandler : BaseActionHandler {
		public override string Name => "Move";

		public override bool IsPotentialPossible(EcsEntity characterEntity, EcsEntity locationEntity) =>
			characterEntity.Get<Character>().CurrentLocation != locationEntity.Get<Location>().Id;

		public override void Perform(EcsEntity characterEntity, EcsEntity locationEntity) {
			ref var moveAction = ref characterEntity.Get<MoveCharacterActionEvent>();
			moveAction.TargetLocation = locationEntity.Get<Location>().Id;
		}
	}
}