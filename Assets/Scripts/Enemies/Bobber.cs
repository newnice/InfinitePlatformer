using UnityEngine;

namespace Enemies {
    public class Bobber : Enemy, IsAirEnemy {
        [SerializeField] private float _minPosition = -5f;
        [SerializeField] private float _movementRange = 10f;
        [SerializeField] private float UpVelocityScale = 15f;

        private float _maxPosition;
        private Vector2 _direction;

        protected virtual void OnEnable() {
            _minPosition = Random.Range(_minPosition, 8f);
            _maxPosition = _minPosition+_movementRange;
            transform.position = new Vector3(transform.position.x, _minPosition);
            _direction = Vector2.up;
        }

        protected virtual void FixedUpdate() {
            if (!IsAlive()) return;
            if (transform.position.y >= _maxPosition)
                _direction = Vector2.down;
            else if (transform.position.y <= _minPosition) {
                _direction = Vector2.up;
            }

            EnemyRigidBody.velocity = _direction * UpVelocityScale;
        }
    }
}