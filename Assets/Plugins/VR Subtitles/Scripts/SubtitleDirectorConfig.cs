using UnityEngine;

namespace VRSubtitles {
	[CreateAssetMenu(menuName = "Subtitle Config")]
	public class SubtitleDirectorConfig : ScriptableObject {
		[SerializeField] private SubtitleUI m_Template;
		[Tooltip("Used to calculate the automatic duration of subtitles if no duration is given.")]
		[SerializeField, Range(150, 200)] private int m_WordsPerMinute = 175;
		[SerializeField, Range(0, 10)] private float m_MinimumDuration = 3;
		[SerializeField, Range(0, 5)] private float m_FadeIn = 1;
		[SerializeField] private AnimationCurve m_FadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
		[SerializeField, Range(0, 5)] private float m_FadeOut = 1;
		[SerializeField] private AnimationCurve m_FadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
		[SerializeField, Range(500, 2000)] private int m_MaxSubtitleWidth = 800;
		[SerializeField, Range(100, 1000)] private int m_MaxPortraitWidth = 200;
		[SerializeField, Range(100, 1000)] private int m_MaxPortraitHeight = 200;

		public SubtitleUI Template { get { return m_Template; } }
		public float WPM {  get { return m_WordsPerMinute; } }
		public float MinimumDuration {  get { return m_MinimumDuration; } }
		public float FadeIn { get { return m_FadeIn; } }
		public AnimationCurve FadeInCurve { get { return m_FadeInCurve; } }
		public float FadeOut { get { return m_FadeOut; } }
		public AnimationCurve FadeOutCurve { get { return m_FadeOutCurve; } }
		public int MaxSubtitleWidth { get { return m_MaxSubtitleWidth; } }
		public int MaxPortraitWidth { get { return m_MaxPortraitWidth; } }
		public int MaxPortraitHeight { get { return m_MaxPortraitHeight; } }

		[Space, Header("Stay In FOV Settings")]
		[SerializeField] private float m_DistanceFromCamera = 1.3f;
		[SerializeField, Range(1, 10)] private float m_FovMoveSpeed = 2f;
		[Tooltip("The degrees of rotation off of the center when the follower will start re-centering. 0 = dead center.")]
		[SerializeField, Range(0, 90)] private float m_FovAngle = 40;

		public float DistanceFromCamera { get { return m_DistanceFromCamera; } }
		public float FOVMoveSpeed { get { return m_FovMoveSpeed; } }
		public float FOVAngle { get { return m_FovAngle; } }

	}
}