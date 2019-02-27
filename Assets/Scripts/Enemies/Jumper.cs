using UnityEngine;

namespace Enemies {
    public class Jumper : GroundEnemy {
        [SerializeField] private float UpVelocityScale = 15f;
        [SerializeField] private float GravityScale = 2f;

        protected virtual void FixedUpdate() {
            if (onGround)
                EnemyRigidBody.velocity = Vector2.up * UpVelocityScale;
            else 
                EnemyRigidBody.AddForce(Vector2.down*9.8f*GravityScale);
          
        }
    }
}