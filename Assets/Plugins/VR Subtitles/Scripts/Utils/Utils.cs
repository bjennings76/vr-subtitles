using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VRSubtitles.Utils {
	public static class Utils {
		public static bool IsNullOrEmpty(this string s) { return string.IsNullOrEmpty(s); }

		public static void ForEach<T>(this IEnumerable<T> list, Action<T> action) {
			// Try List foreach.
			List<T> listList = list as List<T>;

			if (listList != null) {
				listList.ForEach(action);
				return;
			}

			// Try Array foreach.
			T[] array = list as T[];

			if (array != null) {
				Array.ForEach(array, action);
				return;
			}

			// Use the ol' foreach on IEnumerable if nothing else works.
			foreach (T item in list) { action(item); }
		}

		public static string AggregateToString<T>(this IEnumerable<T> list, string delimiter = ", ", Func<T, string> convert = null) {
			convert = convert ?? (a => a.ToString());
			T[] e = list as T[] ?? list.ToArray();
			return e.Any() ? e.Select(item => convert(item)).Aggregate((a, b) => a + delimiter + b) : "";
		}

		public static int IndexOf<T>(this T[] array, T item) { return Array.IndexOf(array, item); }

		public static bool Approximately(this float n1, float n2, float tolerance = float.Epsilon) { return Math.Abs(n1 - n2) < tolerance; }

		public static T GetOrAddComponent<T>(this Component component) where T : Component { return GetOrAddComponent<T>(component.gameObject); }

		public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
			T component = go.GetComponent<T>();
			return !component ? go.AddComponent<T>() : component;
		}

		public static void DestroyAllChildren(this Transform transform) {
			if (!transform) { return; }

			for (int i = transform.childCount - 1; i >= 0; i--) {
				Transform t = transform.GetChild(i);
				if (!t) { continue; }
				DestroyObject(t);
			}
		}

		public static void DestroyObject(Object obj) {
			GameObject go = GetGameObject(obj);
			if (!go) { return; }
			if (Application.isPlaying) { Object.Destroy(go); }
			else { Object.DestroyImmediate(go); }
		}

		private static GameObject GetGameObject(Object thing) {
			Transform t = thing as Transform;
			if (t) { return t.gameObject; }
			Component c = thing as Component;
			if (c) { return c.gameObject; }
			GameObject go = thing as GameObject;
			return go ? go : null;
		}
	}
}