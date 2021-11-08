using EconomicsGame.Components;
using Leopotam.Ecs;
using UniRx;

namespace EconomicsGame.Services {
	public sealed class RuntimeData {
		public PersistantService PersistantService { get; }
		public IdFactory IdFactory { get; } = new IdFactory();
		// TODO: use single class provider
		public ComponentProvider<Item> ItemProvider { get; } = new ComponentProvider<Item>();
		public ComponentProvider<Character> CharacterProvider { get; } = new ComponentProvider<Character>();
		public ComponentProvider<Location> LocationProvider { get; } = new ComponentProvider<Location>();

		public ReactiveCollection<EcsEntity> Locations { get; } = new ReactiveCollection<EcsEntity>();
		public ReactiveCollection<EcsEntity> Characters { get; } = new ReactiveCollection<EcsEntity>();
		public ReactiveProperty<EcsEntity> SelectedLocation { get; } = new ReactiveProperty<EcsEntity>();
		public ReactiveProperty<EcsEntity> SelectedCharacter { get; } = new ReactiveProperty<EcsEntity>();

		public RuntimeData(IStore store) {
			PersistantService = new PersistantService(store);
		}
	}
}