using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class GroundEnemy : Enemy {
    [SerializeField] private float GravityScale = 2f;

    public bool _onGround;
    public bool onGround => _onGround;

    protected virtual void FixedUpdate() {
        if (!IsAlive()) return;
        var oldOnGround = onGround;
        CheckGroundByRaycast();
        if (oldOnGround && !onGround)
            OnLooseGround();
        else if (onGround)
            OnGroundMovement();
        else {
            OnFall();
        }
    }

    protected virtual void OnLooseGround() {
        //Nothing to do
    }

    protected abstract void OnGroundMovement();

    protected virtual void OnFall() {
        EnemyRigidBody.AddForce(Vector2.down * 9.8f * GravityScale);
    }

    private void CheckGroundByRaycast() {
        RaycastHit2D[] rays = new RaycastHit2D[5];
        var rayCount = EnemyCollider.Raycast(Vector2.down, rays);
        var colliderSize = EnemyCollider.radius * EnemyCollider.transform.localScale.y;
        _onGround = rayCount > 0 && rays.Any(r =>
                        r.collider != null
                        && r.collider.CompareTag(TagNames.GROUND)
                        && r.distance < colliderSize + 0.1f);
    }

    protected override IEnumerator DestroyEnemy() {
        _onGround = false;
        return base.DestroyEnemy();
    }
}