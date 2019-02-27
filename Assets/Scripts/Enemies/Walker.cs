using System.Linq;
using UnityEngine;


public class Walker : GroundEnemy {
    private Vector2 _moveDirection;
    [SerializeField] private float ForwardVelocityScale = 2f;


    private void Start() {
        _moveDirection = Mathf.Sign(Random.Range(-1f, 1f)) * Vector2.left;
    }

    protected virtual void FixedUpdate() {
        if (!onGround) return;
        EnemyRigidBody.velocity = _moveDirection * ForwardVelocityScale;
    }

    private void ChangeDirection() {
        _moveDirection = -1 * _moveDirection;
        EnemyRigidBody.velocity = 2 * _moveDirection * ForwardVelocityScale;
    }

    protected override void OnCollisionEnter2D(Collision2D other) {
        base.OnCollisionEnter2D(other);
        CheckObstacle(other);
    }

    protected override void OnCollisionExit2D(Collision2D other) {
        var oldGroundValue = onGround;
        base.OnCollisionExit2D(other);
        if (oldGroundValue && !onGround)
            ChangeDirection();
    }

    private void CheckObstacle(Collision2D other) {
        var isObstacle =
            other.contacts.Any(contact => Vector2.Dot(_moveDirection, contact.normal) < -0.9f);
        if (isObstacle)
            ChangeDirection();
    }
}