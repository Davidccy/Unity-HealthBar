using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthTenchiwokurau2 : UIHealthBase {
    // NOTE:
    // ìVínÇãÚÇÁÇ§2 ê‘ï«ÇÃêÌÇ¢ = Warriors of Fate

    #region Serialized Fields
    [SerializeField] private RectTransform _rectRoot = null;
    [SerializeField] private RectTransform _rectFrame = null;

    [SerializeField] private Image _imageValueUpper = null;
    [SerializeField] private Image _imageValueLower = null;

    // Now this value can be changed in play mode via slider
    //[SerializeField] private int _rollingSpeed = 10;

    [SerializeField] private Color[] _colors;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI _textDebugRealHealth = null;
    [SerializeField] private TextMeshProUGUI _textDebugDisplayHealth = null;
    #endregion

    #region Internal Fields
    private float _maxWidth;

    private bool _updateNeeded = false;
    private bool _skipRolling = false;

    private int _displayHealth;
    private int _targetHealth;
    #endregion

    #region Mono Behaviour Hooks
    private void Update() {
        if (!_updateNeeded) {
            return;
        }

        UpdateDisplay();
    }
    #endregion

    #region Override Methods
    public override void OnAwake() {
        _maxWidth = _rectRoot.rect.width;
    }

    public override void OnDebugModeChanged() {
        UpdateDebugModeRoot();
    }

    public override void OnValueUpdated(bool isInit, int oldHealth, int newHealth, int maxValue) {
        _maxHealth = maxValue;
        _curHealth = newHealth;

        UpdateFrame();
        CalculateTargetHealth();

        _updateNeeded = true;
        _skipRolling = isInit;
    }
    #endregion

    #region Internal Fields
    private void UpdateDisplay() {
        CalculateDisplayHealth();
        UpdateDebugModeValue();

        // Health bar upper
        int healthUpperColorIndex = _displayHealth / _valuePerLine;
        int fillValueUpper = _displayHealth % _valuePerLine;
        Color colorUpper = GetColor(healthUpperColorIndex);

        // NOTE:
        // AnchorMin = (0, 0)
        // AnchorMax = (1, 1)
        // Pivot = (0, 0.5)
        //
        // SizeDelta = (0, 0) when full health

        _imageValueUpper.rectTransform.sizeDelta = new Vector2(-_maxWidth * (1 - (float) fillValueUpper / _valuePerLine), _imageValueUpper.rectTransform.sizeDelta.y);
        _imageValueUpper.color = colorUpper;

        // Health bar lower
        int healthLowerColorIndex = healthUpperColorIndex - 1;
        int fillValueLower;
        Color colorLower = Color.white;
        if (healthLowerColorIndex < 0) {
            fillValueLower = 0;
        }
        else {
            colorLower = GetColor(healthLowerColorIndex);
            fillValueLower = _valuePerLine;
        }

        // NOTE:
        // AnchorMin = (0, 0)
        // AnchorMax = (1, 1)
        // Pivot = (0, 0.5)
        //
        // SizeDelta = (0, 0) when full health

        _imageValueLower.rectTransform.sizeDelta = new Vector2(-_maxWidth * (1 - (float) fillValueLower / _valuePerLine), _imageValueLower.rectTransform.sizeDelta.y);
        _imageValueLower.color = colorLower;
    }

    private void UpdateDebugModeRoot() {
        _textDebugRealHealth.gameObject.SetActive(OptionSettings.DebugMode);
        _textDebugDisplayHealth.gameObject.SetActive(OptionSettings.DebugMode);
    }

    private void UpdateDebugModeValue() {
        _textDebugRealHealth.text = string.Format("Real: {0} / {1}", _curHealth, _maxHealth);
        _textDebugDisplayHealth.text = string.Format("Display: {0}", _displayHealth);
    }

    private void UpdateFrame() {
        int maxHealthLineIndex = GetLineIndex(_maxHealth);
        int curHealthLineIndex = GetLineIndex(_curHealth);

        // NOTE:
        // AnchorMin = (0, 0)
        // AnchorMax = (1, 1)
        // Pivot = (0, 0.5)
        //
        // SizeDelta = (0, 0) when full health

        int fillValue = (maxHealthLineIndex == curHealthLineIndex) && (maxHealthLineIndex == 0) ? _maxHealth : _valuePerLine;
        _rectFrame.sizeDelta = new Vector2(-_maxWidth * (1 - (float) fillValue / _valuePerLine), _rectFrame.sizeDelta.y);
    }

    private void CalculateTargetHealth() {
        _targetHealth = _curHealth;
        _targetHealth = Mathf.Min(_targetHealth, _colors.Length * _valuePerLine);
    }

    private void CalculateDisplayHealth() {
        // NOTE:
        // '_displayHealth' chases '_targetHealth'

        if (_skipRolling) {
            _displayHealth = _targetHealth;

            _updateNeeded = false;
            _skipRolling = false;
        }
        else {
            if (_displayHealth > _targetHealth) {
                if (_displayHealth - _targetHealth < OptionSettings.HealthRollingSpeed) {
                    _displayHealth = _targetHealth;
                }
                else {
                    _displayHealth -= OptionSettings.HealthRollingSpeed;
                }
            }
            else if (_targetHealth > _displayHealth) {
                if (_targetHealth - _displayHealth < OptionSettings.HealthRollingSpeed) {
                    _displayHealth = _targetHealth;
                }
                else {
                    _displayHealth += OptionSettings.HealthRollingSpeed;
                }
            }

            if (_displayHealth == _targetHealth) {
                _updateNeeded = false;
                _skipRolling = false;
            }
        }
    }

    private Color GetColor(int lineIndex) {
        if (_colors == null || _colors.Length <= 0) {
            return Color.red;
        }

        if (lineIndex >= _colors.Length) {
            return _colors[_colors.Length - 1];
        }

        return _colors[lineIndex];
    }
    #endregion
}
