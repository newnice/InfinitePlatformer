using UnityEngine;

namespace Enemies {
    public class Jumper : GroundEnemy {
        [SerializeField] private float UpVelocityScale = 15f;

        protected override void OnGroundMovement() {
            EnemyRigidBody.velocity = Vector2.up * UpVelocityScale;
        }
    }
}