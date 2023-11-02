using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthDNF : UIHealthBase {
    // NOTE:
    // Dungeon & Fighter = DNF = ÉAÉâÉhêÌãL = Dungeon Fighter Online = DFO

    public class BarData {
        public int LineIndex;
        public int Order;
        public RectTransform RectRoot;
    }

    #region Serialized Fields
    [SerializeField] private RectTransform _rectRoot = null;
    [SerializeField] private RectTransform _rectFrame = null;

    [SerializeField] private TextMeshProUGUI _textLineCount = null;

    // Now this value can be changed in play mode via slider
    //[SerializeField] private float _rollingSpeed = 10;

    [SerializeField] private List<RectTransform> _rectRootBrightList = null;
    [SerializeField] private List<Image> _imageBrightList = null;
    [SerializeField] private List<RectTransform> _rectRootDarkList = null;
    [SerializeField] private List<Image> _imageDarkList = null;

    [SerializeField] private UIHealthDNFDecreaseEffect _uiDecEffectRes = null;

    [Header("Shaking")]
    [SerializeField] private float _shakingTime = 0.5f;
    [SerializeField] private float _shakingWave = 10;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI _textDebugRealHealth = null;
    [SerializeField] private TextMeshProUGUI _textDebugDisplayHealth = null;
    #endregion

    #region Internal Fields
    private List<BarData> _barDataList = new List<BarData>();

    private float _maxWidth;

    private bool _updateNeeded = false;
    private bool _skipRolling = false;

    private float _displayHealthBright;
    private int _targetHealthBright;

    private float _displayHealthDark;
    private int _targetHealthDark;

    private float _shakingRemainTime = 0;
    private RectTransform _rect;
    private Vector3 _oriAnchoredPosition;
    #endregion

    #region Mono Behaviour Hooks
    private void Update() {
        DamageShaking();

        if (!_updateNeeded) {
            return;
        }

        UpdateDisplay();
    }
    #endregion

    #region Override Methods
    public override void OnAwake() {
        _maxWidth = _rectRoot.rect.width;

        _rect = transform as RectTransform;
        _oriAnchoredPosition = _rect.anchoredPosition;
    }

    public override void OnDebugModeChanged() {
        UpdateDebugModeRoot();
    }

    public override void OnValueUpdated(bool isInit, int oldHealth, int newHealth, int maxHealth) {
        _maxHealth = maxHealth;
        _curHealth = newHealth;

        UpdateFrame();

        if (isInit) {
            _targetHealthBright = _curHealth;
            _targetHealthDark = _curHealth;
        }
        else {
            _targetHealthBright = _curHealth;
            GenerateEffectObject(oldHealth, newHealth);

            if (OptionSettings.DNFShaking) {
                _shakingRemainTime = _shakingTime;
            }
        }

        _updateNeeded = true;
        _skipRolling = isInit;
    }
    #endregion

    #region Internal Fields
    private void UpdateDebugModeRoot() {
        _textDebugRealHealth.gameObject.SetActive(OptionSettings.DebugMode);
        _textDebugDisplayHealth.gameObject.SetActive(OptionSettings.DebugMode);
    }

    private void UpdateDebugModeValue() {
        _textDebugRealHealth.text = string.Format("Real: {0} / {1}", _curHealth, _maxHealth);
        _textDebugDisplayHealth.text = string.Format("Display: {0}", _displayHealthDark);
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

    private void GenerateEffectObject(int oldValue, int newValue) {
        // NOTE:

        // Case - 2345 -> 2100
        // Generate 1 object
        // One from 100 to 345 on Layer 3

        // Case - 2345 -> 1753
        // Geterate 2 object 
        // One from 0 to 345 on Layer 3
        // Another one from 753 to 1000 On Layer 2

        // Health Decrease Effect Object
        int lineIndexOld = GetLineIndex(oldValue);
        int lineIndexNew = GetLineIndex(newValue);

        int progressValue = oldValue;
        int targetValue = newValue;

        int layerCount = _rectRootDarkList.Count;
        int handledValue = 0;
        int preventCount = 0;

        List<(UIHealthDNFDecreaseEffect, Color)> effectList = new List<(UIHealthDNFDecreaseEffect, Color)>();
        while (progressValue > targetValue && handledValue < layerCount * _valuePerLine) {
            int lineIndexProgress = GetLineIndex(progressValue);

            int handleValueFrom = Mathf.Min(GetMaxValueInLine(lineIndexProgress), progressValue);
            int handleValueTo = Mathf.Max(GetMinValueInLine(lineIndexProgress), targetValue);

            int objValueFrom = (handleValueFrom % _valuePerLine == 0) ? _valuePerLine : handleValueFrom % _valuePerLine;
            int objValueTo = handleValueTo % _valuePerLine;

            int layerIndex = lineIndexProgress % _rectRootDarkList.Count;
            UIHealthDNFDecreaseEffect newEffect = Instantiate(_uiDecEffectRes, _rectRootDarkList[layerIndex]);
            Color color = _imageDarkList[layerIndex].color;
            RectTransform newEffectRect = newEffect.transform as RectTransform;
            newEffectRect.gameObject.SetActive(true);

            newEffectRect.anchoredPosition = new Vector2(_maxWidth * objValueTo / _valuePerLine, newEffectRect.anchoredPosition.y);
            newEffectRect.sizeDelta = new Vector2(_maxWidth * (objValueFrom - objValueTo) / _valuePerLine, newEffectRect.sizeDelta.y);

            newEffect.transform.SetAsLastSibling();

            effectList.Add((newEffect, color));

            progressValue = handleValueTo;
            handledValue += (handleValueFrom - handleValueTo);

            preventCount += 1;
            if (preventCount > 10) {
                Debug.LogErrorFormat("Inifite loop occurs");
                break;
            }
        }

        for (int i = 0; i < effectList.Count; i++) {
            UIHealthDNFDecreaseEffect effect = effectList[i].Item1;
            Color color = effectList[i].Item2;

            if (i != effectList.Count - 1) {
                effect.StartCountDown(DecreaseEffectCallback, 0.4f, -1, color);
            }
            else {
                // Final effect
                effect.StartCountDown(DecreaseEffectCallback, 0.4f, newValue, color);
            }
        }
    }

    private void DecreaseEffectCallback(UIHealthDNFDecreaseEffect effect) {
        GameObject.Destroy(effect.gameObject);

        if (effect.ToHealthValue == -1) {
            return;
        }

        if (_targetHealthDark <= effect.ToHealthValue) {
            return;
        }

        _targetHealthDark = effect.ToHealthValue;
        _updateNeeded = true;
    }

    private void UpdateDisplay() {
        CalculateDisplayHealth();
        UpdateDebugModeValue();

        _barDataList.Clear();

        // Switch Layer Method
        int layerCount = _imageBrightList.Count;

        // Bright Layer
        int brightTopLineIndex = GetLineIndex(_displayHealthBright);        
        int brightTopLayerIndex = brightTopLineIndex % layerCount;
        for (int i = 0; i < layerCount; i++) {
            int lineIndex = brightTopLineIndex - i;

            int layerIndex = (brightTopLayerIndex - i) % layerCount;
            if (layerIndex < 0) {
                layerIndex += layerCount;
            }

            float fillValue = 0;
            if (lineIndex == brightTopLineIndex) {
                fillValue = GetFillValue(_displayHealthBright);
            }
            else {
                fillValue = lineIndex >= 0 ? _valuePerLine : 0;
            }

            // NOTE:
            // AnchorMin = (0, 0)
            // AnchorMax = (1, 1)
            // Pivot = (0, 0.5)
            //
            // SizeDelta = (0, 0) when full health

            _imageBrightList[layerIndex].rectTransform.sizeDelta = new Vector2(-_maxWidth * (1 - fillValue / _valuePerLine), _imageBrightList[layerIndex].rectTransform.sizeDelta.y);

            _barDataList.Add(new BarData { LineIndex = lineIndex, Order = lineIndex * 2, RectRoot = _rectRootBrightList[layerIndex] });
        }

        // Dark Layer
        int darkTopLineIndex = GetLineIndex(_displayHealthDark);
        int darkTopLayerIndex = darkTopLineIndex % layerCount;
        for (int i = 0; i < layerCount; i++) {
            int lineIndex = darkTopLineIndex - i;

            int layerIndex = (darkTopLayerIndex - i) % layerCount;
            if (layerIndex < 0) {
                layerIndex += layerCount;
            }

            float fillValue = 0;
            if (lineIndex == darkTopLineIndex) {
                fillValue = GetFillValue(_displayHealthDark);
            }
            else {
                fillValue = lineIndex >= 0 ? _valuePerLine : 0;
            }

            // NOTE:
            // AnchorMin = (0, 0)
            // AnchorMax = (1, 1)
            // Pivot = (0, 0.5)
            //
            // SizeDelta = (0, 0) when full health

            _imageDarkList[layerIndex].rectTransform.sizeDelta = new Vector2(-_maxWidth * (1 - fillValue / _valuePerLine), _imageDarkList[layerIndex].rectTransform.sizeDelta.y);

            _barDataList.Add(new BarData { LineIndex = lineIndex, Order = lineIndex * 2 - 1, RectRoot = _rectRootDarkList[layerIndex] });
        }

        // Sorting by order value
        _barDataList.Sort(new BarDataSortingMethod());
        for (int i = 0; i < _barDataList.Count; i++) {
            _barDataList[i].RectRoot.transform.SetAsLastSibling();
        }

        ShowLineConut(darkTopLineIndex + 1);
    }

    private void CalculateDisplayHealth() {
        //Debug.LogErrorFormat("CalculateDisplayHealth");
        _displayHealthBright = _targetHealthBright;

        // NOTE:
        // '_displayHealth' chases '_targetHealth'

        if (_skipRolling) {
            _displayHealthDark = _targetHealthDark;

            _updateNeeded = false;
            _skipRolling = false;
        }
        else {
            if (_displayHealthDark > _targetHealthDark) {
                if (_displayHealthDark - _targetHealthDark < OptionSettings.HealthRollingSpeed) {
                    _displayHealthDark = _targetHealthDark;
                }
                else {
                    _displayHealthDark -= OptionSettings.HealthRollingSpeed;
                }
            }
            else if (_targetHealthDark > _displayHealthDark) {
                _displayHealthDark = _targetHealthDark;
            }

            if (_displayHealthDark == _targetHealthDark) {
                _updateNeeded = false;
                _skipRolling = false;
            }
        }
    }

    private void ShowLineConut(int lineCount) {
        if (lineCount <= 1) {
            _textLineCount.text = string.Empty;
        }
        else {
            _textLineCount.text = string.Format("x {0}", lineCount);
        }
    }

    private float GetFillValue(float healthValue) {
        float fillValue = healthValue % _valuePerLine;
        if (healthValue >= _valuePerLine && fillValue % _valuePerLine == 0) {
            fillValue = _valuePerLine;
        }

        return fillValue;
    }

    private void DamageShaking() {
        if (_shakingRemainTime <= 0) {
            _rect.anchoredPosition = _oriAnchoredPosition;
            return;
        }

        _rect.anchoredPosition = new Vector3(_oriAnchoredPosition.x + Random.Range(-_shakingWave, _shakingWave), _oriAnchoredPosition.y + Random.Range(-_shakingWave, _shakingWave), 0);

        _shakingRemainTime -= Time.deltaTime;
    }

    public class BarDataSortingMethod : IComparer<BarData> {
        public int Compare(BarData x, BarData y) {
            return x.Order.CompareTo(y.Order); ;
        }
    }
    #endregion
}
