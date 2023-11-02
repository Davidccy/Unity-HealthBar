using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Enemy _enemyRes = null;
    [SerializeField] private RectTransform _rectEnemyRoot = null;
    [SerializeField] private float _respawnCountDownMax = 1.0f; // Seconds
    [SerializeField] private float _respawnCountDownMin = 0.5f; // Seconds
    #endregion

    #region Internal Fields
    private Enemy _curEnemy = null;
    private float _rndDuration = -1; // Seconds
    #endregion

    #region Mono Behaviour Hooks
    private void Update() {
        TrySpawnEnemy();
    }
    #endregion

    #region Internal Methods
    private void TrySpawnEnemy() {
        if (_curEnemy != null) {
            return;
        }

        if (_curEnemy == null && _rndDuration == -1) {
            _rndDuration = Random.Range(_respawnCountDownMin, _respawnCountDownMax);
        }

        if (_rndDuration > 0) {
            _rndDuration -= Time.deltaTime;
            return;
        }

        _curEnemy = Instantiate(_enemyRes, _rectEnemyRoot);
        _curEnemy.gameObject.SetActive(true);

        int rndEnemyMaxHealth = Random.Range(10000, 15000);
        _curEnemy.Init(rndEnemyMaxHealth, OnEnemyDefeated);

        AttackHandler.RegisterEnemy(_curEnemy);
    }

    private void OnEnemyDefeated(Enemy e) {
        Destroy(e.gameObject);

        _curEnemy = null;
        _rndDuration = -1;
    }
    #endregion
}
