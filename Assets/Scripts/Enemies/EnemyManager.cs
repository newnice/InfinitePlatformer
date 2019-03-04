using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies {
    [Serializable]
    public struct EnemySpawnType {
        public Enemy _enemyPrefab;
        public float _spawnChance;
    }

    public class EnemyManager : MonoBehaviour {
        private List<Enemy> _enemies;
        [SerializeField] private int _maxPoolSize = 20;
        [SerializeField] private List<EnemySpawnType> _spawnConf = null;
        [SerializeField] private int _maxHealth = 3;


        private Dictionary<int, int> _healthMap;
        private readonly int HealthWeight1Step = 5;

        private void Awake() {
            InitHealthGenerator();
            InitEnemiesPool();
        }

        private void InitHealthGenerator() {
            _healthMap = new Dictionary<int, int>();
            var weightSum = 0;
            var weight1Step = HealthWeight1Step;
            var health = _maxHealth;
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
                var count = (int) (_maxPoolSize * conf._spawnChance / sumChance);
                if (k == _spawnConf.Count - 1) count = _maxPoolSize - spawned;
                for (var i = 0; i < count; i++) {
                    var enemy = Instantiate(conf._enemyPrefab);
                    enemy.gameObject.SetActive(false);
                    _enemies.Add(enemy);
                }

                spawned += count;
            }
        }

        private int GenerateRandomHealth() {
            var random = Random.Range(HealthWeight1Step, _healthMap.Keys.Sum());
            return _healthMap[_healthMap.Keys.Where(k => k <= random).Max()];
        }

        public void SpawnAirEnemy(Vector2 position, float range, bool isMandatory = false) {
            if (isMandatory)
                StartCoroutine("WaitFreeAirEnemy");
            var nonActiveAir = _enemies.Where(e => !e.isActiveAndEnabled && e is IsAirEnemy);
            var count = nonActiveAir.Count();
            if (count == 0) return;


            var enemy = nonActiveAir.ToArray()[Random.Range(0, count - 1)];
            enemy.Renew(position, range, GenerateRandomHealth());
        }

        private IEnumerator WaitFreeAirEnemy() {
            int count;
            do {
                var nonActiveAir = _enemies.Where(e => !e.isActiveAndEnabled && e is IsAirEnemy);
                count = nonActiveAir.Count();
                if (count == 0) yield return new WaitForSeconds(1f);
            } while (count == 0);
        }

        public void SpawnGroundEnemy(Vector3 spawnPoint) {
            var nonActive = _enemies.Where(e => !e.isActiveAndEnabled && !(e is IsAirEnemy));
            var enemies = nonActive.ToArray();
            if (enemies.Length == 0) return;

            var enemy = enemies[Random.Range(0, enemies.Length - 1)];
            enemy.Renew(spawnPoint, 1f, GenerateRandomHealth());
        }
    }
}