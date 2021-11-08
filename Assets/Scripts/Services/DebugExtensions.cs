using EconomicsGame.Components;

namespace EconomicsGame.Services {
	public static class DebugExtensions {
		public static string Log(this ref Location location) => $"{location.Name} (LOC:{location.Id})";
		public static string Log(this ref Character character) => $"{character.Name} (CH:{character.Id})";
		public static string Log(this ref Item item) => $"{item.Name} (IT:{item.Id})";
	}
}