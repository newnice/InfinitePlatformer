using System.Collections;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private readonly int MaxHealth = 1;
    private int _health = 1;
    private Rigidbody2D rb;

    protected int Health => _health;

    protected Rigidbody2D Rb => rb;

    protected CircleCollider2D CircleColl => circleColl;

    private CircleCollider2D circleColl;
    private SpriteRenderer sr;
    [SerializeField] private readonly float VelocityScale = 8f;
    [SerializeField] private Color _originColor;
    private bool _onGround;

    public bool onGround => _onGround;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        circleColl = GetComponent<CircleCollider2D>();
        _health = MaxHealth;
    }

    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("KillZone")) {
            gameObject.SetActive(false);
        }
    }

    public bool Squash() {
        _health--;
        return _health<=0;
    }

    protected void OnCollisionEnter2D(Collision2D other) {
        CheckGround(other);
    }

    private bool CheckGround(Collision2D other) {
        if (!other.gameObject.CompareTag("Ground")) return false;

        _onGround = other.contacts.Any(c => Vector2.Dot(c.normal.normalized, Vector2.up) > 0.9f);

        return true;
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ground")) {
            _onGround = false;
        }
    }

    private void OnBecameInvisible() {
        gameObject.SetActive(false);
    }


    protected void Update() {
        if (_health <= 0)
            StartCoroutine("DestroyEnemy");
    }

    private IEnumerator DestroyEnemy() {
        _onGround = false;
        circleColl.isTrigger = true;
        sr.color = Color.gray;
        rb.velocity = Vector2.down * VelocityScale;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    public void Renew(Vector2 position) {
        _onGround = false;
        _health = MaxHealth;
        circleColl.isTrigger = false;
        sr.color = Color.red;
        gameObject.transform.position = position;
        rb.velocity = Vector2.down * VelocityScale;
        gameObject.SetActive(true);
    }
}