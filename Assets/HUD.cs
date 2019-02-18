using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    [SerializeField] private Text _livesText;
    [SerializeField] private Text _enemiesText;
    [SerializeField] private Text _scoreText;

    public void UpdateLives(int lives) {
        _livesText.text = $"Lives: {(lives >= 0 ? lives : 0)}";
    }

    public void UpdateEnemies(int enemies) {
        _enemiesText.text = $"Enemies: {(enemies >= 0 ? enemies : 0)}";
    }

    public void UpdateScore(int score) {
        _scoreText.text = $"Score: {(score >= 0 ? score : 0)}";
    }
}