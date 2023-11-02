using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthStreetOfRage2 : UIHealthBase {
    // NOTE:
    // ベア．ナックル = Bare Knuckle = Streets of Rage

    #region Serialized Fields
    [SerializeField] private RectTransform _rectRoot = null;
    [SerializeField] private RectTransform _rectFrame = null;

    [SerializeField] private Image  _imageValue = null;

    // Now this value can be changed in play mode via slider
    //[SerializeField] private int _rollingSpeed = 10;

    [SerializeField] private Color _colorExtra;
    [SerializeField] private Color _colorNormal;

    [Header("Stars")]
    [SerializeField] private int _maxStarDisplayCount = 5;

    [SerializeField] private GameObject _goStarTypeAllImage = null;
    [SerializeField] private GameObject _goStarRes = null;
    [SerializeField] private RectTransform _rectStarRoot = null;
    
    [SerializeField] private GameObject _goStarTypeText = null;
    [SerializeField] private TextMeshProUGUI _textStarCount = null;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI _textDebugRealHealth = null;
    [SerializeField] private TextMeshProUGUI _textDebugDisplayHealth = null;
    #endregion

    #region Internal Fields
    private float _maxWidth;
    private int _frameValue;

    private bool _updateNeeded;
    private bool _skipRolling;

    private int _displayHealth;
    private int _targetHealth;
    private List<GameObject> _goStarList = new List<GameObject>();
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
        UpdateStars();        

        CalculateTargetHealth();

        _updateNeeded = true;
        _skipRolling = isInit;
    }

    #endregion

    #region Internal Methods
    private void UpdateDisplay() {
        CalculateDisplayHealth();
        UpdateDebugModeValue();

        // NOTE:
        // AnchorMin = (0, 0)
        // AnchorMax = (1, 1)
        // Pivot = (0, 0.5)
        //
        // SizeDelta = (0, 0) when full health

        _imageValue.rectTransform.sizeDelta = new Vector2(-_maxWidth * (1 - (float) _displayHealth / _valuePerLine), _rectFrame.sizeDelta.y);
        _imageValue.color = _curHealth > _valuePerLine ? _colorExtra : _colorNormal;
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

        _frameValue = maxHealthLineIndex == curHealthLineIndex ? _maxHealth % _valuePerLine : _valuePerLine;
        _rectFrame.sizeDelta = new Vector2(-_maxWidth * (1 - (float) _frameValue / _valuePerLine), _rectFrame.sizeDelta.y);
    }

    private void UpdateStars() {
        int starNeeded = _curHealth / _valuePerLine;
        if (_curHealth >= _valuePerLine && _curHealth % _valuePerLine == 0) {
            starNeeded -= 1;
        }

        _goStarTypeAllImage.SetActive(starNeeded <= _maxStarDisplayCount);
        _goStarTypeText.SetActive(starNeeded > _maxStarDisplayCount);

        if (starNeeded <= _maxStarDisplayCount) {
            for (int i = 0; i < starNeeded; i++) {
                if (_goStarList.Count <= i) {
                    GameObject goNewStar = GameObject.Instantiate(_goStarRes, _rectStarRoot.transform);
                    _goStarList.Add(goNewStar);
                }

                _goStarList[i].SetActive(true);
            }

            for (int i = starNeeded; i < _goStarList.Count; i++) {
                _goStarList[i].SetActive(false);
            }
        }
        else {
            _textStarCount.text = string.Format("x {0}", starNeeded);
        }
    }

    private void CalculateTargetHealth() {
        _targetHealth = _curHealth % _valuePerLine;
    }

    private void CalculateDisplayHealth() {
        // NOTE:
        // '_displayHealth' chases '_targetHealth'
        
        if (_curHealth >= _valuePerLine && _targetHealth == 0) {
            _targetHealth = _valuePerLine;
        }

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

        _displayHealth = Mathf.Clamp(_displayHealth, 0, _frameValue);
    }
    #endregion
}
