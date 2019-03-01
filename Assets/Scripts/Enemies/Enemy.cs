using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private int _health = 1;
    private Rigidbody2D _rb;

    protected Rigidbody2D EnemyRigidBody => _rb;

    private CircleCollider2D _enemyColl;
    protected CircleCollider2D EnemyCollider => _enemyColl;
    private SpriteRenderer sr;
    [SerializeField] private readonly float DeathVelocityScale = 8f;
    [SerializeField] private Color _originColor = Color.black;

    private Vector3 _defaultScale;


    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        _enemyColl = GetComponent<CircleCollider2D>();
        _defaultScale = gameObject.transform.localScale;
    }

    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(TagNames.KILLZONE)) {
            BecomeInvisible();
        }
    }

    private void Update() {
        if (!IsAlive())
            StartCoroutine("DestroyEnemy");
    }

    public bool Squash() {
        _health--;
        UpdateSize();
        return !IsAlive();
    }

    private void UpdateSize() {
        if (IsAlive())
            gameObject.transform.localScale = _defaultScale + new Vector3(_health, _health, 0);
    }

    public bool IsAlive() {
        return _health > 0;
    }

    private void BecomeInvisible() {
        gameObject.SetActive(false);
    }
    
    protected virtual void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.CompareTag(TagNames.MAIN_CAMERA)) {
            BecomeInvisible();
        }
    }


    protected virtual IEnumerator DestroyEnemy() {
        _enemyColl.isTrigger = true;
        sr.color = Color.gray;
        _rb.velocity = Vector2.down * DeathVelocityScale;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    public void Renew(Vector2 position, int health) {
        _health = health;
        _enemyColl.isTrigger = false;
        sr.color = _originColor;

        gameObject.transform.position = position;
        UpdateSize();
        _rb.velocity = Vector2.down * DeathVelocityScale;
        gameObject.SetActive(true);
    }
}