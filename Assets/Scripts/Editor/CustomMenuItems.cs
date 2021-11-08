using EconomicsGame.Services;
using UnityEditor;

namespace EconomicsGame.Editor {
	public static class CustomMenuItems {
		[MenuItem("EconomicsGame/OpenSave")]
		public static void OpenSave() {
			EditorUtility.OpenWithDefaultApp(new PersistentDataFileStore().SaveRoot);
		}

		[MenuItem("EconomicsGame/DeleteSave")]
		public static void DeleteSave() {
			new PersistentDataFileStore().Delete();
		}
	}
}