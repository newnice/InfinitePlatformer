using Common;
using UnityEngine;

namespace Enemies {
    public class Orbiter : Enemy, IsAirEnemy {
        [SerializeField] private float _minOrbitRadius = 1f;
        [SerializeField] private float _angleSpeed = Mathf.PI / 240;
        private Vector3 _center;
        private float _angle;
        private float _orbitRadius;

        protected virtual void OnEnable() {
            _center = transform.position;
            if (_orbitRadius.FloatEquals(0f))
                _orbitRadius = _minOrbitRadius;

        }

        protected override bool TrySetRequiredWidth(float availableWidth) {
            if (availableWidth / 2 < _minOrbitRadius) return false;
            _orbitRadius = Random.Range(_minOrbitRadius, availableWidth / 2);
            Debug.Log($"min width = {availableWidth}");
            return true;
        }

        protected virtual void FixedUpdate() {
            if (!IsAlive()) return;
            _angle += _angleSpeed;

            var positionDif = new Vector3(_orbitRadius * Mathf.Cos(_angle), _orbitRadius * Mathf.Sin(_angle), 0);
            EnemyRigidBody.position = _center + positionDif;
        }
    }
}