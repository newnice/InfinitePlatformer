using UnityEngine;

public class PlayerCharacter : MonoBehaviour {
    [SerializeField] private HUD _hud=null;
    private Rigidbody2D _rb;

    private int _lives = GameplayConstants.STARTING_LIVES;
    private int _distanceScore = 0;
    private int _enemyScore = 0;

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
        UpdateScore();
        _hud.UpdateLives(_lives);
        _hud.UpdateEnemies(_enemyScore);
    }

    void Update() {
        UpdateScore();
    }


    private void UpdateScore() {
        _distanceScore = Mathf.Max(_distanceScore, (int) transform.position.x);
        int totalScore = _distanceScore * GameplayConstants.SCORE_DISTANCE_MULTIPLIER +
                         _enemyScore * GameplayConstants.SCORE_ENEMY_MULTIPLIER;
        _hud.UpdateScore(totalScore);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag(TagNames.KILLZONE)) {
            KillCharacter();
        }
    }

    protected void OnCollisionEnter2D(Collision2D other) {
        CheckEnemy(other);
    }

    private void CheckEnemy(Collision2D other) {
        if (!other.gameObject.CompareTag(TagNames.ENEMY)) return;
        if (Vector2.Dot(Vector2.up, other.relativeVelocity.normalized) > 0.8) {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy.Squash()) {
                _enemyScore++;
                _hud.UpdateEnemies(_enemyScore);
            }
        }
    }

    private void KillCharacter() {
        _lives--;
        _hud.UpdateLives(_lives);

        if (_lives > 0) {
            _rb.MovePosition(_rb.position + GameplayConstants.RESPAWN_HEIGHT * Vector2.up);
            _rb.velocity = Vector2.zero;
        }
        else {
            GameOver();
        }
    }

    private void GameOver() {
        _rb.isKinematic = true;
        _rb.velocity = Vector2.zero;
        Debug.Log("Game Over!");
    }
}