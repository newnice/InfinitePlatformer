using UnityEngine;

namespace Enemies {
    public class Flapper : Enemy, IsAirEnemy {
        [SerializeField] private float _minPosition = -1f;
        [SerializeField] private float _upVelocityScale = 15f;

        protected virtual void OnEnable() {
            _minPosition = Random.Range(_minPosition, _minPosition + 3);
            _upVelocityScale = Random.Range(_upVelocityScale - 2, _upVelocityScale + 2);
            transform.position = new Vector3(transform.position.x, _minPosition);
        }

        protected virtual void FixedUpdate() {
            if (!IsAlive()) return;
            if (transform.position.y <= _minPosition) {
                EnemyRigidBody.velocity = Vector2.up * _upVelocityScale;
            }
        }
    }
}