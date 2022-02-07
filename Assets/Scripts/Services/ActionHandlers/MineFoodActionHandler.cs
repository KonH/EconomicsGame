using EconomicsGame.Components;
using Leopotam.Ecs;

namespace EconomicsGame.Services.ActionHandlers {
	public sealed class MineFoodActionHandler : BaseActionHandler {
		public override string Name => "Mine Food";

		public override bool IsPotentialPossible(EcsEntity characterEntity, EcsEntity locationEntity) {
			if ( characterEntity.Get<Character>().CurrentLocation != locationEntity.Get<Location>().Id ) {
				return false;
			}
			if ( !locationEntity.Has<FoodSource>() ) {
				return false;
			}
			ref var source = ref locationEntity.Get<FoodSource>();
			return (source.Remaining > 0) && (source.Locked < source.Remaining);
		}

		public override void Perform(EcsEntity characterEntity, EcsEntity locationEntity) {
			ref var mineFoodAction = ref characterEntity.Get<MineFoodCharacterActionEvent>();
			mineFoodAction.TargetLocation = locationEntity.Get<Location>().Id;
		}
	}
}