using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthDNFDecreaseEffect : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Image _image = null;
    #endregion

    #region Internal Fields
    private Action<UIHealthDNFDecreaseEffect> _callback = null;
    private float _countDownMax = 0;
    private float _countDown = 0;
    private int _toHealthValue = 0;
    private Color _targetColor;

    private bool _updateCountDown = false;
    #endregion

    #region Mono Behaviour Hooks
    private void Update() {
        if (!_updateCountDown) {
            return;
        }

        UpdateCountDown();
    }
    #endregion

    #region Properties
    public int ToHealthValue {
        get {
            return _toHealthValue;
        }
    }
    #endregion

    #region APIs
    public void StartCountDown(Action<UIHealthDNFDecreaseEffect> cb, float countDown, int toHealthValue, Color targetColor) {
        _callback = cb;
        _countDownMax = countDown;
        _countDown = countDown;
        _toHealthValue = toHealthValue;

        //_targetColor = targetColor;
        _targetColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0);

        _updateCountDown = true;
    }
    #endregion

    #region Internal Methods
    private void UpdateCountDown() {
        if (_countDown <= 0 && _callback != null) {
            _callback(this);
            _callback = null;

            _updateCountDown = false;
        }

        _countDown -= Time.deltaTime;

        float lerpValue = _countDownMax == 0 ? 1 : (1 - _countDown / _countDownMax);
        _image.color = Color.Lerp(Color.white, _targetColor, lerpValue);
    }
    #endregion
}
