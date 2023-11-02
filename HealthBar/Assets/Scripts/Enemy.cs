using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private RectTransform _goDamageValueEffectRoot = null;
    [SerializeField] private UIDamageValueEffect _goEffectRes = null;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI _textDebugRealHealth = null;
    #endregion

    #region Internal Fields
    private int _maxHealth = 0;
    private int _curHealth = 0;
    private Action<Enemy> _cb = null;

    private float _shakingRemainTime = 0;
    private RectTransform _rect;
    private Vector3 _oriAnchoredPosition;
    #endregion

    #region Properties
    private int MaxHealth {
        get {
            return _maxHealth;
        }
    }

    private int CurHealth {
        get {
            return _curHealth;
        }
    }
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _rect = transform as RectTransform;
        _oriAnchoredPosition = _rect.anchoredPosition;

        UpdateDebugModeRoot();

        Utility.RegisterDebugModeCallback(this, (bool b) => UpdateDebugModeRoot());
    }

    private void Update() {
        DamageShaking();
    }

    private void OnDestroy() {
        Utility.UnregisterDebugModeCallback(this, (bool b) => UpdateDebugModeRoot());
    }
    #endregion

    #region APIs
    public void Init(int maxHealth, Action<Enemy> cb) {
        _maxHealth = maxHealth;
        _curHealth = maxHealth;
        _cb = cb;

        UIHealthHandler.EnemyHealthChanged(true, _curHealth, _curHealth, _maxHealth);

        UpdateDebugModeRoot();
        UpdateDebugModeValue();
    }

    public void OnDamaged(int damageValue) {
        int oldHealth = _curHealth;

        _curHealth -= damageValue;
        _curHealth = Mathf.Clamp(_curHealth, 0, _maxHealth);

        if (oldHealth == _curHealth) {
            return;
        }

        GenerateDamageValueEffect(oldHealth - _curHealth);

        _shakingRemainTime = 0.15f;

        UIHealthHandler.EnemyHealthChanged(false, oldHealth, _curHealth, _maxHealth);

        UpdateDebugModeValue();

        if (_curHealth <= 0) {
            OnDefeated();
        }
    }
    #endregion

    #region Internal Methods
    private void OnDefeated() {
        if (_cb == null) {
            return;
        }

        _cb(this);
    }

    private void UpdateDebugModeRoot() {
        _textDebugRealHealth.gameObject.SetActive(OptionSettings.DebugMode);
    }

    private void UpdateDebugModeValue() {
        _textDebugRealHealth.text = string.Format("{0} / {1}", _curHealth, _maxHealth);
    }

    private void DamageShaking() {
        if (_shakingRemainTime <= 0) {
            _rect.anchoredPosition = _oriAnchoredPosition;
            return;
        }

        _rect.anchoredPosition = new Vector3(_oriAnchoredPosition.x + Random.Range(-5f, 5f), _oriAnchoredPosition.y + Random.Range(-5f, 5f), 0);

        _shakingRemainTime -= Time.deltaTime;
    }

    private void GenerateDamageValueEffect(int value) {
        if (!OptionSettings.DamageValueEffect) {
            return;
        }

        UIDamageValueEffect newEffect = Instantiate(_goEffectRes, _goDamageValueEffectRoot);
        Vector3 effectPosition = this.transform.position + new Vector3(0, 90, 0);
        newEffect.transform.position = effectPosition;
        newEffect.gameObject.SetActive(true);
        newEffect.StartPerform(value);
    }
    #endregion
}
