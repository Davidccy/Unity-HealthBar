using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthFinalFight : UIHealthBase {
    // NOTE:
    // ファイナルファイト = Final Fight

    #region Serialized Fields
    [SerializeField] private RectTransform _rectRoot = null;
    [SerializeField] private RectTransform _rectFrame = null;

    [SerializeField] private Image _imageValue = null;
    [SerializeField] private Color[] _colors;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI _textDebugRealHealth = null;
    #endregion

    #region Internal Fields
    private float _maxWidth;
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
        UpdateDisplay();
        UpdateDebugModeValue();
    }
    #endregion

    #region Internal Fields
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

    private void UpdateDisplay() {
        int lineIndex = GetLineIndex(_curHealth);
        int fillValue = GetFillValue(_curHealth);
        Color color = GetColor(lineIndex);

        // NOTE:
        // AnchorMin = (0, 0)
        // AnchorMax = (1, 1)
        // Pivot = (0, 0.5)
        //
        // SizeDelta = (0, 0) when full health

        _imageValue.rectTransform.sizeDelta = new Vector2(-_maxWidth * (1 - (float) fillValue / _valuePerLine), _imageValue.rectTransform.sizeDelta.y);
        _imageValue.color = color;
    }

    private void UpdateDebugModeRoot() {
        _textDebugRealHealth.gameObject.SetActive(OptionSettings.DebugMode);
    }

    private void UpdateDebugModeValue() {
        _textDebugRealHealth.text = string.Format("Real: {0} / {1}", _curHealth, _maxHealth);
    }

    private int GetFillValue(int healthValue) {
        // Method 1
        //int lineIndex = GetLineIndex(healthValue);
        //if (lineIndex > 0) {
        //    return _healthPerLine;
        //}

        //return Mathf.Max(0, healthValue); ;

        // Method 2
        if (healthValue >= _valuePerLine) {
            return _valuePerLine;
        }

        return Mathf.Max(0, healthValue);
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
