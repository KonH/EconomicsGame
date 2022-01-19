using EconomicsGame.Services;
using Leopotam.Ecs;

namespace EconomicsGame.Tests {
	static class Services {
		public static LocationService CreateLocationService(EcsWorld world, EntityProvider entityProvider) =>
			new LocationService(
				world,
				new IdFactory(),
				entityProvider);

		public static CharacterService CreateCharacterService(EcsWorld world, EntityProvider entityProvider) =>
			new CharacterService(
				world,
				new IdFactory(),
				entityProvider);

		public static ItemService CreateItemService(EcsWorld world, EntityProvider entityProvider, MarketService marketService) =>
			new ItemService(
				world,
				new IdFactory(),
				entityProvider,
				marketService);
	}
}