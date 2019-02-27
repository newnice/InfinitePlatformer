using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private int _health = 1;
    private Rigidbody2D _rb;

    protected Rigidbody2D EnemyRigidBody => _rb;

    private CircleCollider2D _circleColl;
    protected CircleCollider2D CircleCollider => _circleColl;
    private SpriteRenderer sr;
    [SerializeField] private readonly float DeathVelocityScale = 8f;
    [SerializeField] private Color _originColor = Color.black;

    private Vector3 _defaultScale;


    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        _circleColl = GetComponent<CircleCollider2D>();
        _defaultScale = gameObject.transform.localScale;
    }

    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(TagNames.KILLZONE)) {
            BecomeInvisible();
        }
    }

    private void Update() {
        if (_health <= 0)
            StartCoroutine("DestroyEnemy");
    }

    public bool Squash() {
        _health--;
        UpdateSize();
        return _health <= 0;
    }

    private void UpdateSize() {
        if (_health > 0)
            gameObject.transform.localScale = _defaultScale + new Vector3(_health, _health, 0);
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
        _circleColl.isTrigger = true;
        sr.color = Color.gray;
        _rb.velocity = Vector2.down * DeathVelocityScale;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    public void Renew(Vector2 position, int health) {
        _health = health;
        _circleColl.isTrigger = false;
        sr.color = _originColor;

        gameObject.transform.position = position;
        UpdateSize();
        _rb.velocity = Vector2.down * DeathVelocityScale;
        gameObject.SetActive(true);
    }
}