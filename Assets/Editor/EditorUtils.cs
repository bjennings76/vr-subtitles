using UnityEditor;
using UnityEngine;

namespace VRSubtitles.Utils {
	public static class EditorUtils {

		[MenuItem("File/Save Project Shortcut %&#s")]
		public static void SaveProject() {
			AssetDatabase.SaveAssets();
			Debug.Log("Project saved.");
		}
	}
}