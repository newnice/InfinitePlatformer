using System.Collections;
using System.Linq;
using UnityEngine;


public class Walker : Enemy {
    private Vector2 moveDirection;
    [SerializeField] private float VelocityScale = 2f;
    [SerializeField] private float CollisionCheckRadius = 0.1f;

    private bool _isObstacle;

    private void Start() {
        moveDirection = Mathf.Sign(Random.Range(-1f, 1f)) * Vector2.left;
    }

    protected void FixedUpdate() {
        StartCoroutine("MakeStep");
    }

    private IEnumerator MakeStep() {
        if (!onGround) yield break;
        Rb.velocity = moveDirection * VelocityScale;
        yield return new WaitForFixedUpdate();

        if (!onGround || _isObstacle) {
            moveDirection = -1 * moveDirection;
            Rb.velocity = 2 * moveDirection * VelocityScale;
            _isObstacle = false;
        }
    }

    protected void OnCollisionEnter2D(Collision2D other) {
        base.OnCollisionEnter2D(other);
        CheckObstacle(other);
    }

    private void CheckObstacle(Collision2D other) {
        _isObstacle = other.contacts.Any(contact => Vector2.Dot(moveDirection, contact.normal) < -0.9f);
    }
}