using Common;
using UnityEngine;

namespace Player {
    public class PlayerUserMovement : MonoBehaviour {
        [SerializeField] private bool _autoRun = false;

        private PlayerMovement _movement;
        private bool _isJump;
        private float _moveDir = 1f;
        private AutoRunMovement _autoRunMovement;


        // Start is called before the first frame update
        void Start() {
            _movement = GetComponent<PlayerMovement>();
            
            _autoRunMovement = gameObject.AddComponent<AutoRunMovement>();
            _autoRunMovement.Movement = _movement;
            _autoRunMovement.Active = _autoRun;
        }

        // Update is called once per frame
        void Update() {
            if (!_isJump && !_autoRun)
                _isJump = Input.GetButtonDown("Jump");
        }

        private void FixedUpdate() {
            _autoRunMovement.Active = _autoRun;
            if (_autoRun) return;

            float x = Input.GetAxis("Horizontal");
            _movement.Move(x, _isJump, IsChangeDirection(x));
            _isJump = false;
        }


        private bool IsChangeDirection(float xShift) {
            if (xShift.FloatEquals(0f)) return false;
            var newDir = Mathf.Sign(xShift);
            if (newDir.FloatEquals(_moveDir)) return false;
            _moveDir = newDir;
            return true;
        }
    }
}