using UnityEngine;
using VRSubtitles.Utils;

namespace VRSubtitles {
	public class ShowSubtitle : MonoBehaviour {
		[SerializeField] private string m_Text;
		[SerializeField] private Sprite m_Portrait;
		[SerializeField] private float m_Delay;
		[SerializeField] private float m_Duration = -1;
		[SerializeField] private Transform m_Speaker;

		private Subtitle m_Subtitle;

		private void Awake() {
			if (m_Text.IsNullOrEmpty()) {
				string warning = "WARNING: " + GetScenePath(transform) + " needs it's subtitle text set properly.";
				m_Text = warning;
				Debug.LogWarning(warning, this);
			}
		}

		private void OnEnable() {
			// Delay a frame for each component above us, in case there are other 'ShowSubtitles' above.
			int priority = GetComponents<ShowSubtitle>().IndexOf(this);
			m_Subtitle = SubtitleDirector.Show(m_Text, m_Portrait, m_Delay, m_Duration, priority, null, null, m_Speaker);
		}

		private void OnDisable() { m_Subtitle.Complete(); }

		private static string GetScenePath(Transform t) {
			string path = null;
			while (t) {
				path = path == null ? t.name : t.name + "/" + path;
				t = t.parent;
			}

			return path;
		}
	}
}