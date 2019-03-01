using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct EnemySpawnType {
    public Enemy _enemyPrefab;
    public bool isAir;
    public float _spawnChance;
    public int _maxHealth;
}

public class EnemyManager : MonoBehaviour {
    private List<Enemy> _enemies;

    [SerializeField] private int MaxPoolSize = 20;
    [SerializeField] private int MaxActiveEnemies = 5;

    [SerializeField] private List<EnemySpawnType> _spawnConf = null;

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
        var health = 5; //MaxHealth;
        while (health > 0) {
            weightSum += weight1Step;
            _healthMap.Add(weightSum, health);
            health--;
            weight1Step *= 3;
        }
    }

    private void InitEnemiesPool() {
        _enemies = new List<Enemy>();
        var sumChance = _spawnConf.Sum(t => t._spawnChance);

        var spawned = 0;
        for (var k = 0; k < _spawnConf.Count; k++) {
            var conf = _spawnConf[k];
            var count = (int) (MaxPoolSize * conf._spawnChance / sumChance);
            if (k == _spawnConf.Count - 1) count = MaxPoolSize - spawned;
            for (var i = 0; i < count; i++) {
                var enemy = Instantiate(conf._enemyPrefab);
                enemy.gameObject.SetActive(false);
                _enemies.Add(enemy);
            }

            spawned += count;
        }
    }

    private void Update() {
        var active = _enemies.Count(e => e.isActiveAndEnabled);
        if (active >= MaxActiveEnemies) return;

        if (TryGenerateNewPosition(out var position)) {
            var enemy = _enemies.Where(e => !e.isActiveAndEnabled).ToArray() [Random.Range(0, MaxPoolSize-active)];
            enemy.Renew(position, GenerateRandomHealth());
        }
    }

    private int GenerateRandomHealth() {
        var random = Random.Range(HealthWeight1Step, _healthMap.Keys.Sum());
        return _healthMap[_healthMap.Keys.Where(k => k <= random).Max()];
    }

    private bool TryGenerateNewPosition(out Vector2 enemyPosition) {
        enemyPosition = new Vector2(Random.Range(1f, 2.2f), 1.5f);
        Vector2 startPos = _camera.ViewportToWorldPoint(enemyPosition);


        var raycast = Physics2D.RaycastAll(startPos, Vector2.down).Where(r => r.collider.CompareTag(TagNames.GROUND))
            .ToArray();

        if (raycast != null && raycast.Length > 0) {
            enemyPosition = raycast[Random.Range(0, raycast.Length - 1)].point;
            return true;
        }

        return false;
    }
}