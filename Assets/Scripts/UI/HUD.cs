﻿using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    [SerializeField] private Text _livesText = null;
    [SerializeField] private Text _enemiesText = null;
    [SerializeField] private Text _scoreText = null;
    [SerializeField] private Text _timerText = null;

    [SerializeField] private GameLanguage _language = GameLanguage.Empty;


    public void UpdateLives(int lives) {
        _livesText.text =
            $"{LocalizationManager.TranslateOrDefault(_language, "UI.LivesText", "Lives")}: { lives}";
    }

    public void UpdateEnemies(int enemies) {
        _enemiesText.text =
            $"{LocalizationManager.TranslateOrDefault(_language, "UI.EnemiesText", "Enemies")}: {enemies }";
    }

    public void UpdateScore(int score) {
        _scoreText.text =
            $"{LocalizationManager.TranslateOrDefault(_language, "UI.ScoreText", "Score")}: {score }";
    }

    public void UpdateTotalAutorunTime(float totalTime) {
        _timerText.text = $"{LocalizationManager.TranslateOrDefault(_language, "UI.TotalTimeText", "TotalTime")}: {totalTime:F0}";
    }
}