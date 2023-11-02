using TMPro;
using UnityEngine;

public class UIDamageValueEffect : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private TextMeshProUGUI _textValue = null;
    [SerializeField] private float _maxVerticalMovement = 20;
    [SerializeField] private float _randomHorizontalOffset = 5;
    [SerializeField] private AnimationCurve _aniCurvePosition = null;
    #endregion

    #region internal Fields
    private Vector3 _oriPosition;
    private bool _updateCountDown = false;
    private float _countDownMax = 0.8f;
    private float _countDown = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _oriPosition = this.transform.position;
    }

    private void Update() {
        if (!_updateCountDown) {
            return;
        }

        UpdateDisplay();
    }
    #endregion

    #region APIs
    public void StartPerform(int value) {
        _countDown = _countDownMax;
        _textValue.text = string.Format("{0}", value);

        float rndHorOffset = Random.Range(-_randomHorizontalOffset, _randomHorizontalOffset);
        _oriPosition = new Vector3(_oriPosition.x + rndHorOffset, _oriPosition.y, _oriPosition.z);

        _updateCountDown = true;
    }
    #endregion

    #region Internal Methods
    private void UpdateDisplay() {
        _countDown -= Time.deltaTime;

        float lerpValue = _aniCurvePosition.Evaluate(Mathf.Max(0, (1 - _countDown / _countDownMax)));

        _textValue.transform.position = new Vector3(_oriPosition.x, _oriPosition.y + lerpValue * _maxVerticalMovement, _oriPosition.z);
        //_image.color = Color.Lerp(Color.white, _targetColor, lerpValue);

        if (_countDown <= 0) {
            SetfDestroy();
        }
    }

    private void SetfDestroy() {
        GameObject.Destroy(this.gameObject);
    }
    #endregion
}
