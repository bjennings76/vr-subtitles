using UnityEngine;
using VRTK;

namespace VRSubtitles.Utils {
	public class StayInFov : MonoBehaviour {
		[SerializeField] private Transform m_Player;
		[SerializeField] private float m_Distance = 1.3f;
		[SerializeField, Range(1, 10)] private float m_Speed = 2f;

		[Tooltip("The degrees of rotation off of the center when the follower will start re-centering. 0 = dead center."), SerializeField, Range(0, 90)]
		private float m_RecenterAngle = 40;

		[SerializeField] private Transform m_Roost;
		[SerializeField] private Vector3 m_RoostOffset;

		public float Distance { private get { return m_Distance; } set { m_Distance = value; } }
		public float Speed { private get { return m_Speed; } set { m_Speed = value; } }
		public float RecenterAngle { private get { return m_RecenterAngle; } set { m_RecenterAngle = value; } }

		private Vector3 m_FreePosition;
		private bool m_SubtitleCentered;
		private bool m_RoostCentered;
		private Vector3 m_LastTargetPosition;
		private Vector3 m_OriginalScale;
		private bool m_Init;

		private Transform Player { get { return m_Player ? m_Player : (m_Player = VRTK_DeviceFinder.HeadsetTransform()); } }

		public Transform Roost { set { m_Roost = value; } }

		private Vector3 TargetPosition {
			get {
				if (m_RoostCentered) { return m_Roost.position + m_Player.localRotation * m_RoostOffset; }

				if (m_SubtitleCentered) { return m_LastTargetPosition; }

				return m_LastTargetPosition = Player.position + new Vector3(Player.forward.x, 0, Player.forward.z).normalized * Distance;
			}
		}

		private void Start() { m_OriginalScale = transform.localScale; }

		private void Update() {
			if (!m_Init) {
				if (!Player) { return; }

				m_Init = true;
				m_FreePosition = transform.position = TargetPosition;
			}

			UpdateFovState(m_Roost, ref m_RoostCentered);
			UpdateFovState(transform, ref m_SubtitleCentered);
		}

		private void UpdateFovState(Transform target, ref bool wasCentered) {
			if (!target) {
				wasCentered = false;
				return;
			}

			float angle = Vector3.Angle(target.position - Player.position, Player.forward);
			if (!InCenter(angle) && wasCentered) { wasCentered = false; }
			else if (InCenter(angle, 0.5f) && !wasCentered) { wasCentered = true; }
		}

		private void LateUpdate() {
			if (!Player || !m_Init) { return; }

			m_FreePosition = Vector3.Lerp(m_FreePosition, TargetPosition, Speed * Time.deltaTime);
			Vector3 direction = m_FreePosition - Player.position;
			transform.position = Player.position + direction.normalized * Distance;
			transform.localScale = m_OriginalScale * Distance;
		}

		private bool InCenter(float angle, float scale = 1) { return angle < RecenterAngle * scale; }

		private void OnDrawGizmosSelected() { Gizmos.DrawSphere(TargetPosition, 0.02f); }
	}
}