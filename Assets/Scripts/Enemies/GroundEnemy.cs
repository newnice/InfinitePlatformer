using System.Collections;
using System.Linq;
using UnityEngine;

public class GroundEnemy : Enemy {
    private bool _onGround;
    public bool onGround => _onGround;


    protected override void OnCollisionEnter2D(Collision2D other) {
        base.OnCollisionEnter2D(other);
        CheckGround(other);
        if (!_onGround) CheckGroundByRaycast();
    }

    private void CheckGround(Collision2D other) {
        if (other.collider.CompareTag(TagNames.GROUND))
            _onGround = other.contacts.Any(c => Vector2.Dot(c.normal.normalized, Vector2.up) > 0.9f);
    }

    protected virtual void OnCollisionExit2D(Collision2D other) {
        CheckGroundByRaycast();
        Debug.Log($"On collision exit onGround = {_onGround}");
    }

    private void CheckGroundByRaycast() {
        RaycastHit2D[] rays = new RaycastHit2D[5];
        var rayCount = CircleCollider.Raycast(Vector2.down, rays);
        var colliderSize = CircleCollider.radius * CircleCollider.transform.localScale.y;
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