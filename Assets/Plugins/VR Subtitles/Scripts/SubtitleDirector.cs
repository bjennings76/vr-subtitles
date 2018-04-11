using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using VRSubtitles.Utils;
using VRTK;

namespace VRSubtitles {
	public class SubtitleDirector : Singleton<SubtitleDirector> {
		[SerializeField] private SubtitleDirectorConfig m_Config;

		private readonly List<Subtitle> m_SubtitleQueue = new List<Subtitle>();
		private SubtitleUI m_UIInstance;
		private Subtitle m_CurrentSubtitle;
		private Transform m_Player;
		private FaceTarget m_FaceTarget;
		private StayInFov m_StayInFov;

		private SubtitleUI UIInstance { get { return m_UIInstance ? m_UIInstance : (m_UIInstance = Instantiate(m_Config.Template, transform)); } }
		private bool CanShowNext { get { return m_CurrentSubtitle == null || m_CurrentSubtitle.IsComplete; } }
		public static SubtitleDirectorConfig Config { get { return Instance.m_Config ? Instance.m_Config : (Instance.m_Config = ScriptableObject.CreateInstance<SubtitleDirectorConfig>()); } }

		protected SubtitleDirector() { }

		private void Start() {
			m_Config = m_Config ? m_Config : ScriptableObject.CreateInstance<SubtitleDirectorConfig>();
			m_FaceTarget = this.GetOrAddComponent<FaceTarget>();
			m_StayInFov = this.GetOrAddComponent<StayInFov>();
			m_StayInFov.Distance = m_Config.DistanceFromCamera;
			m_StayInFov.Speed = m_Config.FOVMoveSpeed;
			m_StayInFov.RecenterAngle = m_Config.FOVAngle;
		}

		private void Update() {
			CheckForPlayer();
			EvaluateQueue();
		}

		private void CheckForPlayer() {
			if (m_Player) { return; }
			m_Player = VRTK_DeviceFinder.HeadsetTransform();
			m_FaceTarget.Target = m_Player;
		}

		private void EvaluateQueue() {
			if (m_SubtitleQueue.Count > 0 && CanShowNext && m_SubtitleQueue[0].TargetTime <= Time.time) {
				Subtitle subtitle = m_CurrentSubtitle = m_SubtitleQueue[0];
				m_SubtitleQueue.RemoveAt(0);
				ShowNow(subtitle);
			}
		}

		private void ShowNow(Subtitle subtitle) {
			if (subtitle.Template) {
				transform.DestroyAllChildren();
				m_UIInstance = Instantiate(subtitle.Template, transform);
				m_UIInstance.transform.localRotation = Quaternion.identity;
				m_UIInstance.transform.localPosition = Vector3.zero;
			}

			m_StayInFov.Roost = subtitle.Speaker;
			UIInstance.Show(subtitle);
			subtitle.Show();
			Debug.Log("[Subtitles] Showing (" + subtitle.Duration + ") '" + subtitle.Text + "'");
			DelayTracker.DelayAction(() => FadeOut(subtitle), subtitle.Duration);
		}

		/// <summary>
		///   Creates a subtitle from the included options and adds it to the subtitle queue.
		/// </summary>
		/// <param name="text">The text of the subtitle.</param>
		/// <param name="portrait">An optional sprite to be used as the speaker's portrait.</param>
		/// <param name="delay">Any delay desired before the text is shown.</param>
		/// <param name="duration">
		///   The length of time to show the subtitle for. Durations &lt; zero will be automatically
		///   calculated based on words-per-minute and minimum duration settings.
		/// </param>
		/// <param name="priority">Determines ties when the start times are about the same.</param>
		/// <param name="onComplete">Action to take when the subtitle is complete.</param>
		/// <param name="template">Optional SubtitleUI template to use for this subtitle.</param>
		/// <param name="speaker">The transform on which this subtitle wants to 'roost' on if it's in the FOV.</param>
		/// <returns></returns>
		public static Subtitle Show(string text, Sprite portrait = null, float delay = 0, float duration = -1, int priority = 0, Action onComplete = null, SubtitleUI template = null, Transform speaker = null) {
			Subtitle subtitle = new Subtitle(text, portrait, delay, duration, priority, null, onComplete, template, speaker);
			Instance.m_SubtitleQueue.Add(subtitle);
			Instance.m_SubtitleQueue.Sort(SubtitleComparer.Instance);
			return subtitle;
		}

		/// <summary>
		///   Hides the assigned subtitle or removes it from the queue.
		/// </summary>
		/// <param name="callback">Optional callback for when fade out is complete.</param>
		public static void FadeOut(Action callback) { FadeOut(Instance.m_CurrentSubtitle, callback); }

		/// <summary>
		///   Hides the assigned subtitle or removes it from the queue.
		/// </summary>
		/// <param name="subtitle">
		///   If set, this subtitle will be triggered to hide or removed from the queue.
		///   If not set, the current subtitle will be hidden.
		/// </param>
		/// <param name="callback">Optional callback for when fade out is complete.</param>
		public static void FadeOut(Subtitle subtitle = null, Action callback = null) {
			subtitle = subtitle ?? Instance.m_CurrentSubtitle;

			if (subtitle == null) { return; }

			if (Instance.m_SubtitleQueue.Contains(subtitle)) {
				Instance.m_SubtitleQueue.Remove(subtitle);
				subtitle.Cancel();
				if (callback != null) { callback(); }
			}
			else {
				Instance.UIInstance.FadeOut(subtitle, () => {
					if (callback != null) { callback(); }
					subtitle.Complete();
				});
			}
		}

		/// <summary>
		///   Hides the current subtitle and clears the subtitle queue.
		/// </summary>
		public static void Clear() {
			Instance.m_SubtitleQueue.Clear();
			if (Instance.m_CurrentSubtitle != null) { Instance.m_CurrentSubtitle.Complete(); }
		}

		public static float GetDuration(float duration, string text) {
			float wordsPerSecond = Instance.m_Config.WPM / 60;

			// If duration is < 0 (e.g. -1), then calculate the duration based on default words-per-minute.
			if (duration < 0) { duration = Regex.Matches(text, @"\w+").Count / wordsPerSecond; }

			// Make sure the duration meets the minimum duration.
			duration = Mathf.Max(Instance.m_Config.MinimumDuration, duration);

			return duration;
		}
	}
}