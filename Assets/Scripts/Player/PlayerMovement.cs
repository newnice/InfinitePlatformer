﻿using System.Linq;
using Common;
using UnityEngine;

namespace Player {
    public class PlayerMovement : MonoBehaviour {
        private Animator _animator;

        private Rigidbody2D _rb;
        private BoxCollider2D _collider;
        private Transform _groundCheck;

        private bool _onGround = true;
        [SerializeField] private float _groundCheckRadius = 0.25f;
        [SerializeField] private float _speedScale = 10f;
        [SerializeField] private float _jumpForce = 1000f;

  
        public Vector2 CurrentVelocity => _rb.velocity;

        private Vector2 _playerSize;


        private void Start() {
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody2D>();
            _groundCheck = transform.Find("GroundCheck");
            _collider = GetComponent<BoxCollider2D>();
            _playerSize = _collider.size * transform.localScale;
        }

        private void CheckCollisions() {
            CheckGround();
            var isObstacles = CheckObstacles();
            _rb.velocity = new Vector2(!isObstacles ? _rb.velocity.x : 0f, _rb.velocity.y);
        }

        private bool CheckObstacles() {
            var direction = Mathf.Sign(transform.localScale.x);
            var colliders = Physics2D.OverlapBoxAll(
                _collider.transform.position + direction * new Vector3(_playerSize.x / 2, _playerSize.y / 2, 0),
                new Vector2(2 * _groundCheckRadius, _playerSize.y),
                0);

            var sideCheck = colliders.Where(c => c.gameObject != gameObject && !c.isTrigger).ToArray();
            var cp = new ContactPoint2D[5];
            foreach (var c in sideCheck) {
                var cpCount = c.GetContacts(cp);
                for (var i = 0; i < cpCount; i++) {
                    if (Mathf.Abs(Vector3.Dot(cp[i].normal, Vector3.right)) > 0.9f)
                        return true;
                }
            }

            return false;
        }

        private void CheckGround() {
            var colliders = Physics2D.OverlapCircleAll(_groundCheck.position, _groundCheckRadius);
            _onGround = colliders.Any(c => c.gameObject != gameObject && !c.isTrigger);
        }


        public void Move(float x, bool isJump, bool isChangeDirection) {
            if (isJump && _onGround)
                _rb.AddForce(new Vector2(0, _jumpForce));
            else MoveHorizontally(x);

            _rb.velocity = new Vector2(_speedScale * x, Mathf.Min(_rb.velocity.y, 18f));

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
            if (_collider == null) return;
            Gizmos.color = Color.red;
            var direction = Mathf.Sign(transform.localScale.x);
            var center = _collider.transform.position + direction*new Vector3(_playerSize.x / 2, _playerSize.y / 2, 0);

            Gizmos.DrawWireCube(center, new Vector2(2 * _groundCheckRadius, _playerSize.y));
        }
    }
}