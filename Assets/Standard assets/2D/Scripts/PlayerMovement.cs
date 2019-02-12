using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private Animator _animator;

    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private Transform _groundCheck, _sideCheck;

    public bool _onGround = true;
    public float _groundCheckRadius = 0.25f;

    private Vector2 _playerSize;

    void Start() {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _groundCheck = transform.Find("GroundCheck");
        _sideCheck = transform.Find("SideCheck");
        _collider = GetComponent<BoxCollider2D>();
        _playerSize = _collider.size * transform.localScale;
    }

    void FixedUpdate() {
        var colliders = Physics2D.OverlapCircleAll(_groundCheck.position, _groundCheckRadius);
        _onGround = colliders.Any(c => c.gameObject != gameObject && !c.isTrigger);
        
        colliders = Physics2D.OverlapBoxAll(_sideCheck.position, new Vector2(2*_groundCheckRadius,_playerSize.y-_groundCheckRadius), 0);
       
        bool sideCheck = colliders.Any(c => c.gameObject != gameObject && !c.isTrigger);

        _rb.velocity = new Vector2(!sideCheck?_rb.velocity.x:0f, Mathf.Min(_rb.velocity.y, 18f));
    }


    // Update is called once per frame
    public void Move(float x, bool isJump, bool isChangeDirection) {
        if (isJump && _onGround) {
            _rb.AddForce(new Vector2(0, 1000f));

        }
        else if (x != 0f ) {
            _animator.SetFloat("Speed", 1.0f);
            _rb.velocity = new Vector2(10 * x, _rb.velocity.y);

        }
        else {
            _animator.SetFloat("Speed", 0);
        }

    
        _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Min(_rb.velocity.y, 18f));

        if (isChangeDirection) {
            var moveScale = transform.localScale;
            moveScale.x *= -1;
            transform.localScale = moveScale;
        }
    }

    private void OnDrawGizmos() {
        if (_sideCheck == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_sideCheck.position, new Vector2(2*_groundCheckRadius,_playerSize.y-_groundCheckRadius));
    }
}