using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    private List<Enemy> _enemies;

    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private int MaxPoolSize = 5;
    [SerializeField] private int MaxActiveEnemies = 3;

    private Camera _camera;
    private void Awake() {
        _enemies = new List<Enemy>();
        for (var i = 0; i < MaxPoolSize; i++) {
            var enemy = Instantiate(_enemyPrefab);
            enemy.gameObject.SetActive(false);
            
            _enemies.Add(enemy);
        }
        _camera = Camera.main;
    }

    private void Update() {
        var active = _enemies.Count(e => e.isActiveAndEnabled);
        if (active >= MaxActiveEnemies) return;

        var enemy = _enemies.First(e => !e.isActiveAndEnabled);
       
        enemy.Renew(_camera.ViewportToWorldPoint(new Vector2(Random.Range(0.5f, 1.0f), 0.9f)));
    }
}