using System;
using System.Collections.Generic;
using UnityEngine;
using VRSubtitles.Utils;

namespace VRSubtitles {
	[Serializable]
	public class Subtitle {
		private float m_StartTime = -1;
		private readonly SubtitleDirectorConfig m_Config;
		private bool m_IsComplete;

		public event Action OnShow;
		public event Action OnComplete;
		public event Action OnCancel;

		public SubtitleDirectorConfig Config { get { return m_Config; } }

		public string Text { get; private set; }
		public Sprite Portrait { get; private set; }
		public float Duration { get; private set; }
		public float TargetTime { get; private set; }
		public int Priority { get; private set; }
		public SubtitleUI Template { get; private set; }
		public Transform Speaker { get; private set; }

		public float StartTime { private get { return m_StartTime; } set { m_StartTime = value; } }
		public bool IsComplete { get { return m_IsComplete || TimeRemaining.Approximately(0); } }
		public float TimeRemaining {  get { return StartTime < 0 ? 10000 : Mathf.Max(StartTime + Duration - Time.time, 0); } }

		public Subtitle(string text, Sprite portrait = null, float delay = 0, float duration = -1, int priority = 0, SubtitleDirectorConfig config = null, Action onComplete = null, SubtitleUI template = null, Transform speaker = null) {
			m_Config = config ? config : SubtitleDirector.Config;
			Text = text;
			Portrait = portrait;
			TargetTime = Time.time + delay;
			Priority = priority;
			Template = template ? template : SubtitleDirector.Config.Template;
			Speaker = speaker;
			Duration = SubtitleDirector.GetDuration(duration, text);
			OnComplete = onComplete;
		}

		public void Show() {
			StartTime = Time.time;
			RaiseOnShow();
		}

		public void Complete() {
			RaiseOnComplete();
			m_IsComplete = true;
		}

		public void Cancel() {
			m_IsComplete = true;
			RaiseOnCancel();
		}

		private void RaiseOnShow() {
			if (OnShow != null) { OnShow(); }
		}

		private void RaiseOnComplete() {
			if (OnComplete != null) { OnComplete(); }
		}

		private void RaiseOnCancel() {
			if (OnCancel != null) { OnCancel(); }
		}
	}

	public class SubtitleComparer : IComparer<Subtitle> {
		private static SubtitleComparer s_Instance;

		public static SubtitleComparer Instance { get { return s_Instance ?? (s_Instance = new SubtitleComparer()); } }

		private SubtitleComparer() { }

		public int Compare(Subtitle x, Subtitle y) {
			if (x == null && y == null) { return 0; }
			if (x == null) { return 1; }
			if (y == null) { return -1; }
			return x.TargetTime.Approximately(y.TargetTime, 0.001f) ? x.Priority.CompareTo(y.Priority) : x.TargetTime.CompareTo(y.TargetTime);
		}
	}
}