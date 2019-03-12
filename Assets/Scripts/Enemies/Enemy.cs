using System.Collections;
using System.Linq;
using Common;
using Player;
using UnityEngine;

namespace Enemies {
    public class Enemy : MonoBehaviour {
        private int _health = 1;
        private Rigidbody2D _rb;

        protected Rigidbody2D EnemyRigidBody => _rb;

        private CircleCollider2D _enemyColl;
        protected CircleCollider2D EnemyCollider => _enemyColl;
        private SpriteRenderer _sr;
        [SerializeField] private readonly float DeathVelocityScale = 8f;
        [SerializeField] private Color _originColor = Color.black;
        public Vector3 Size => EnemyCollider.radius * EnemyCollider.transform.localScale;

        private Vector3 _defaultScale;
        private PlayerCharacter _player;


        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
            _enemyColl = GetComponent<CircleCollider2D>();
            _defaultScale = gameObject.transform.localScale;
        }

        protected void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag(TagNames.KILLZONE)) {
                BecomeInvisible();
            }
        }

        protected void Update() {
            if (!IsAlive())
                StartCoroutine("DestroyEnemy");
        }

        protected virtual void Start() {
            _player = FindObjectOfType<PlayerCharacter>();
        }

        public bool Squash() {
            _health--;
            UpdateSize();
            StartCoroutine("BeforeDie");
            return !IsAlive();
        }

        private IEnumerator BeforeDie() {
            yield return new WaitForSeconds(0.5f);
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

            if (other.collider.CompareTag(TagNames.PLAYER)) {
                StartCoroutine("KillPlayer");
            }
        }

        protected virtual IEnumerator KillPlayer() {
            var i = 0;
            while (i <= 3) {
                yield return new WaitForSeconds(1);
                i++;
                var playerHere = Physics2D.OverlapCircleAll(EnemyCollider.transform.position, Size.x + 0.1f)
                    .Any(c => c.CompareTag(TagNames.PLAYER));
                if (!playerHere) yield break;
            }

            if (_player != null) _player.KillCharacter();
        }


        protected virtual IEnumerator DestroyEnemy() {
            _enemyColl.isTrigger = true;
            _sr.color = Color.gray;
            _rb.velocity = Vector2.down * DeathVelocityScale;
            yield return new WaitForSeconds(1.5f);
            gameObject.SetActive(false);
        }

        public void Renew(Vector2 position, float availableWidth, int health) {
            if (!TrySetRequiredWidth(availableWidth)) return;
            _health = health;
            _enemyColl.isTrigger = false;
            _sr.color = _originColor;

            gameObject.transform.position = position;
            UpdateSize();
            _rb.velocity = Vector2.down * DeathVelocityScale;
            gameObject.SetActive(true);
        }

        protected virtual bool TrySetRequiredWidth(float availableWidth) {
            return true;
        }
    }
}