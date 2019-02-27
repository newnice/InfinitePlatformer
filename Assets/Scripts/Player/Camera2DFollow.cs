using UnityEngine;

namespace UnityStandardAssets._2D {
    public class Camera2DFollow : MonoBehaviour {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;
        public float minimumHeight = -10f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
        private float minimumDistance;

        // Use this for initialization
        private void Start() {
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;

            CorrectLeftBoundPosition();
        }

        private void CorrectLeftBoundPosition() {
            var collider = GetComponent<BoxCollider2D>();
            if (collider == null) return;
            var camera = GetComponent<Camera>();
            if (camera == null) return;

            collider.offset = new Vector2(-camera.aspect * camera.orthographicSize - collider.size.x / 2, 0);
        }


        // Update is called once per frame
        private void Update() {
            minimumDistance = Mathf.Max(transform.position.x, minimumDistance);
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget) {
                m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
            }
            else {
                m_LookAheadPos =
                    Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
            newPos.x = Mathf.Max(newPos.x, minimumDistance);
            newPos.y = Mathf.Max(newPos.y, minimumHeight);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }
}