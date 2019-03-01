using System.Linq;
using UnityEngine;


public class Walker : GroundEnemy {
    public Vector2 _moveDirection;
    [SerializeField] private float ForwardVelocityScale = 2f;


    protected virtual void Start() {
        _moveDirection = Mathf.Sign(Random.Range(-1f, 1f)) * Vector2.left;
    }

    protected override void OnLooseGround() {
        ChangeDirection();
    }

    protected override void OnGroundMovement() {
        EnemyRigidBody.velocity = CalculateVelocity();
    }

    protected virtual void ChangeDirection() {
        _moveDirection = -1 * _moveDirection;
        EnemyRigidBody.velocity = CalculateVelocity();
    }

    protected virtual Vector2 CalculateVelocity() {
        return 2 * _moveDirection * ForwardVelocityScale;
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) {
        CheckObstacle(other);
    }

    private void CheckObstacle(Collision2D other) {
        var isObstacle =
            other.contacts.Any(contact => Vector2.Dot(_moveDirection, contact.normal) < -0.9f);
        if (isObstacle)
            ChangeDirection();
    }
}