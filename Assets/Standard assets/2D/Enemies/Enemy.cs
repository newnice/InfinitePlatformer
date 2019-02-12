using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private readonly int MaxHealth = 1;
    private int _health = 1;
    private Rigidbody2D rb;
    private CircleCollider2D circleColl;
    private SpriteRenderer sr;
    [SerializeField] private readonly float VelocityScale = 8f;
    [SerializeField] private Color _originColor;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        circleColl = GetComponent<CircleCollider2D>();
        _health = MaxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("KillZone")) {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag("Player")) return;
        if (Vector2.Dot(Vector2.down, other.relativeVelocity.normalized) > 0.9)
            _health--;
    }

    private void OnBecameInvisible() {
        gameObject.SetActive(false);
    }


    private void Update() {
        if (_health <= 0)
            StartCoroutine("DestroyEnemy");
     
    }

    private IEnumerator DestroyEnemy() {
        circleColl.isTrigger = true;
        sr.color = Color.gray;
        rb.velocity = Vector2.down * VelocityScale;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    public void Renew(Vector2 position) {

        _health = MaxHealth;
        circleColl.isTrigger = false;
        sr.color = Color.red;
        gameObject.transform.position = position;
        rb.velocity = Vector2.down * VelocityScale;
        gameObject.SetActive(true);
    }
}