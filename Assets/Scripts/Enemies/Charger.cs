using System.Collections;
using System.Linq;
using UnityEngine;

namespace Enemies {
    public class Charger : Walker {
        [SerializeField] private float _distanceToCheckPlayer = 5f;
        [SerializeField] private float _speedScaleWhenSeePlayer = 2.5f;

        private Transform _playerCheck;

        public bool _isTrackPlayer;
        public Vector2 _playerDirection;

        protected override void Start() {
            base.Start();
            _playerCheck = transform.Find("PlayerCheck");
        }

        protected override void FixedUpdate() {
            CheckPlayer();
            base.FixedUpdate();
        }

        protected override void OnLooseGround() {
            if (!_isTrackPlayer)
                base.OnLooseGround();
        }

        protected override void OnGroundMovement() {
            if (_isTrackPlayer)
                ChangeDirection(_playerDirection);
            else {
                base.OnGroundMovement();
            }
        }

        protected override Vector2 CalculateVelocity() {
            return base.CalculateVelocity() * (_isTrackPlayer ? _speedScaleWhenSeePlayer : 1);
        }

        protected void ChangeDirection(Vector3 aimDirection) {
            if (Vector3.Dot(aimDirection, _moveDirection) < 0)
                _moveDirection = -1 * _moveDirection;
            EnemyRigidBody.velocity = CalculateVelocity();
        }

        private void CheckPlayer() {
            var colliders = Physics2D.OverlapCircleAll(_playerCheck.position, _distanceToCheckPlayer);
            var playerCol = colliders.Where(c => c.CompareTag(TagNames.PLAYER));
            _isTrackPlayer = playerCol.Count() > 0;
            if (_isTrackPlayer) {
                var player = playerCol.First();
                _playerDirection = player.transform.position - _playerCheck.position;
            }
        }

        private void OnDrawGizmos() {
            if (_playerCheck == null)
                return;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_playerCheck.position, _distanceToCheckPlayer);
        }

        protected override IEnumerator DestroyEnemy() {
            _isTrackPlayer = false;
            _playerDirection = Vector2.zero;
            return base.DestroyEnemy();
        }
    }
}