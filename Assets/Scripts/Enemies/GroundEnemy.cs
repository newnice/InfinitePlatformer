using System.Collections;
using System.Linq;
using Common;
using Enemies;
using UnityEngine;

public abstract class GroundEnemy : Enemy {
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
        EnemyRigidBody.AddForce(Vector2.down * 9.8f);
    }

    private void CheckGroundByRaycast() {
        RaycastHit2D[] rays = new RaycastHit2D[5];
        var rayCount = EnemyCollider.Raycast(Vector2.down, rays);
        _onGround = rayCount > 0 && rays.Any(r =>
                        r.collider != null
                        && r.collider.CompareTag(TagNames.GROUND)
                        && r.distance < Size.y + 0.1f);
    }

    protected override IEnumerator DestroyEnemy() {
        _onGround = false;
        return base.DestroyEnemy();
    }
}