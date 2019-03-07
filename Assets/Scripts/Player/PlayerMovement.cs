using System.Linq;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Player {
    public class PlayerMovement : MonoBehaviour {
        private Animator _animator;

        private Rigidbody2D _rb;
        private BoxCollider2D _collider;
        private Transform _groundCheck, _sideCheck;

        public bool onGround = true;
        public float groundCheckRadius = 0.25f;
        public float speedScale = 10f;
        public float jumpForce = 1000f;

        private Vector2 _playerSize;
        public Text velocityText;


        private void Start() {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _groundCheck = transform.Find("GroundCheck");
            _sideCheck = transform.Find("SideCheck");
            _collider = GetComponent<BoxCollider2D>();
            _playerSize = _collider.size * transform.localScale;
        }

        private void CheckCollisions() {
            CheckGround();
            CheckObstacles();
            velocityText.text = $"v:{_rb.velocity}";
        }

        private void CheckObstacles() {
            var colliders = Physics2D.OverlapBoxAll(_sideCheck.position,
                new Vector2(2 * groundCheckRadius, _playerSize.y - groundCheckRadius),
                0);

            var sideCheck = colliders.Any(c => c.gameObject != gameObject && !c.isTrigger);

            _rb.velocity = new Vector2(!sideCheck ? _rb.velocity.x : 0f, _rb.velocity.y);
        }

        private void CheckGround() {
            var colliders = Physics2D.OverlapCircleAll(_groundCheck.position, groundCheckRadius);
            onGround = colliders.Any(c => c.gameObject != gameObject && !c.isTrigger);
        }


        public void Move(float x, bool isJump, bool isChangeDirection) {
            if (isJump && onGround)
                _rb.AddForce(new Vector2(0, jumpForce));
            else MoveHorizontally(x);
            
            _rb.velocity = new Vector2(speedScale * x, Mathf.Min(_rb.velocity.y, 18f));
            
            ChangeDirection(isChangeDirection);
            CheckCollisions();
        }

        private void ChangeDirection(bool isChangeDirection) {
            var transformLocalScale = transform.localScale;
            transformLocalScale.x = isChangeDirection ? -transformLocalScale.x : transformLocalScale.x;
            transform.localScale = transformLocalScale;
        }

        private void MoveHorizontally(float speed) {
            _animator.SetFloat(AnimationParams.SPEED, speed.FloatEquals(0f) ? 0f : 1f);
        }

        private void OnDrawGizmos() {
            if (_sideCheck == null) return;
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(_sideCheck.position, new Vector2(2 * groundCheckRadius, _playerSize.y));
        }
    }
}