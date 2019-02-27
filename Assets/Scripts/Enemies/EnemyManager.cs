using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    private List<Enemy> _enemies;

    [SerializeField] private Enemy _enemyPrefab = null;
    [SerializeField] private int MaxPoolSize = 5;
    [SerializeField] private int MaxActiveEnemies = 3;
    [SerializeField] private readonly int MaxHealth = 5;

    private Camera _camera;

    private Dictionary<int, int> _healthMap;
    private readonly int HealthWeight1Step = 5; 

    private void Awake() {
        InitHealthGenerator();
        InitEnemiesPool();
        _camera = Camera.main;
    }

    private void InitHealthGenerator() {
        _healthMap = new Dictionary<int, int>();
        var weightSum = 0;
        var weight1Step = HealthWeight1Step;
        var health = MaxHealth;
        while (health > 0) {
            weightSum += weight1Step;
            _healthMap.Add(weightSum, health);
            health--;
            weight1Step *= 2;
        }
    }

    private void InitEnemiesPool() {
        _enemies = new List<Enemy>();
        for (var i = 0; i < MaxPoolSize; i++) {
            var enemy = Instantiate(_enemyPrefab);
            enemy.gameObject.SetActive(false);

            _enemies.Add(enemy);
        }
    }

    private void Update() {
        var active = _enemies.Count(e => e.isActiveAndEnabled);
        if (active >= MaxActiveEnemies) return;

        if (TryGenerateNewPosition(out var position)) {
            var enemy = _enemies.First(e => !e.isActiveAndEnabled);
            enemy.Renew(position, GenerateRandomHealth());
        }
    }

    private int GenerateRandomHealth() {
        var random = Random.Range(HealthWeight1Step, _healthMap.Keys.Sum());
        return _healthMap[_healthMap.Keys.Where(k => k <= random).Max()];
    }

    private bool TryGenerateNewPosition(out Vector2 enemyPosition) {
        enemyPosition = new Vector2(Random.Range(1f, 2.5f), 1f);
        Vector2 startPos = _camera.ViewportToWorldPoint(enemyPosition);


        var raycast = Physics2D.RaycastAll(startPos, Vector2.down);
        if (raycast != null && raycast.Length > 0) {
            enemyPosition = raycast[Random.Range(0, raycast.Length - 1)].point;
            return true;
        }

        return false;
    }
}