using System.Collections;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private int _health = 1;
    private Rigidbody2D rb;

    protected Rigidbody2D Rb => rb;

    private CircleCollider2D circleColl;
    private SpriteRenderer sr;
    [SerializeField] private readonly float VelocityScale = 8f;
    [SerializeField] private Color _originColor = Color.black;
    private bool _onGround;

    public bool onGround => _onGround;
    private Vector3 _defaultScale;


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        circleColl = GetComponent<CircleCollider2D>();
        _defaultScale = gameObject.transform.localScale;
    }

    protected void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(TagNames.KILLZONE)) {
            gameObject.SetActive(false);
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

    protected virtual void OnCollisionEnter2D(Collision2D other) {
        CheckGround(other);
        if (!_onGround) CheckGroundByRaycast();
    }

    private void CheckGround(Collision2D other) {
        if (other.collider.CompareTag(TagNames.GROUND))
            _onGround = other.contacts.Any(c => Vector2.Dot(c.normal.normalized, Vector2.up) > 0.9f);
    }

    protected virtual void OnCollisionExit2D(Collision2D other) {
        CheckGroundByRaycast();
    }

    private void CheckGroundByRaycast() {
        RaycastHit2D[] rays = new RaycastHit2D[5];
        var rayCount = circleColl.Raycast(Vector2.down, rays);
        var colliderSize = circleColl.radius * circleColl.transform.localScale.y;
        _onGround = rayCount > 0 && rays.Any(r =>
                        r.collider != null
                        && r.collider.CompareTag(TagNames.GROUND)
                        && r.distance < colliderSize + 0.1f);
    }


    private void OnBecameInvisible() {
        gameObject.SetActive(false);
    }


    private IEnumerator DestroyEnemy() {
        _onGround = false;
        circleColl.isTrigger = true;
        sr.color = Color.gray;
        rb.velocity = Vector2.down * VelocityScale;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    public void Renew(Vector2 position, int health) {
        _onGround = false;
        _health = health;
        circleColl.isTrigger = false;
        sr.color = _originColor;


        gameObject.transform.position = position;

        UpdateSize();
        rb.velocity = Vector2.down * VelocityScale;
        gameObject.SetActive(true);
    }
}