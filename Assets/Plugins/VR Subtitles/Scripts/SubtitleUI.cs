using System;
using UnityEngine;
using UnityEngine.UI;
using VRSubtitles.Utils;

namespace VRSubtitles {
	public class SubtitleUI : MonoBehaviour {
		[SerializeField] private FadeCanvasGroup m_Fade;
		[SerializeField] private Image m_Portrait;
		[SerializeField] private Text m_Text;

		private Subtitle m_Current;
		private bool m_FadingOut;
		private float m_FadeOutDuration;

		private FadeCanvasGroup Fade { get { return m_Fade ? m_Fade : (m_Fade = this.GetOrAddComponent<FadeCanvasGroup>()); } }
		private Image Portrait { get { return m_Portrait ? m_Portrait : (m_Portrait = GetComponentInChildren<Image>()); } }
		private Text Text { get { return m_Text ? m_Text : (m_Text = GetComponentInChildren<Text>()); } }

		private void Update() { CheckForFadeOut(); }

		private void OnDestroy() { Unsubscribe(); }

		private void CheckForFadeOut() {
			if (m_Current == null) { return; }

			if (!m_FadingOut && m_Current.TimeRemaining <= m_FadeOutDuration) {
				m_FadingOut = true;
				FadeOut();
			}
		}

		private void Subscribe() {
			if (m_Current == null) return;
			m_Current.OnCancel += OnSubtitleCancel;
			m_Current.OnComplete += OnSubtitleComplete;
		}

		private void Unsubscribe() {
			if (m_Current == null) return;
			m_Current.OnCancel -= OnSubtitleCancel;
			m_Current.OnComplete -= OnSubtitleComplete;
		}

		private void OnSubtitleComplete() { Complete(m_Current); }

		private void OnSubtitleCancel() { Complete(m_Current); }

		public void Show(Subtitle item) {
			Unsubscribe();
			m_Current = item;
			Subscribe();
			m_FadingOut = false;
			m_FadeOutDuration = Mathf.Min(m_Current.Config.FadeOut, item.Duration * 0.4f);
			Fade.In(m_Current.Config.FadeIn, m_Current.Config.FadeInCurve);

			Text.text = item.Text;

			if (Portrait) {
				Portrait.sprite = item.Portrait;
				Portrait.gameObject.SetActive(item.Portrait);
			}
		}

		private void FadeOut(Action callback = null) {
			Subtitle item = m_Current;
			Fade.Out(m_FadeOutDuration, m_Current.Config.FadeOutCurve, () => Complete(item, callback));
		}

		public void FadeOut(Subtitle item, Action callback) {
			if (m_Current != item) { return; }
			FadeOut(callback);
		}

		private void Complete(Subtitle item, Action callback = null) {
			if (m_Current != item) { return; }
			m_Current = null;
			m_FadingOut = false;
			if (callback != null) { callback(); }
		}
	}
}