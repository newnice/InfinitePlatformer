using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    [SerializeField] private Text _livesText;
    [SerializeField] private Text _enemiesText;
    [SerializeField] private Text _scoreText;

    [SerializeField] private GameLanguage _language;


    public void UpdateLives(int lives) {
        _livesText.text = $"{LocalizationManager.TranslateFor(_language, "UI.LivesText")}: {(lives >= 0 ? lives : 0)}";
    }

    public void UpdateEnemies(int enemies) {
        _enemiesText.text =
            $"{LocalizationManager.TranslateFor(_language, "UI.EnemiesText")}: {(enemies >= 0 ? enemies : 0)}";
    }

    public void UpdateScore(int score) {
        _scoreText.text = $"{LocalizationManager.TranslateFor(_language, "UI.ScoreText")}: {(score >= 0 ? score : 0)}";
    }
}