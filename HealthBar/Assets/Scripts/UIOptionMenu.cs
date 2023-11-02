using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionMenu : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Toggle _toggleDebugMode = null;
    [SerializeField] private Toggle _toggleDNFShaking = null;
    [SerializeField] private Toggle _toggleDamageValueEffect = null;

    [SerializeField] private TextMeshProUGUI _textRollingSpeed = null;
    [SerializeField] private Slider _sliderRollingSpeed = null;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _toggleDebugMode.isOn = OptionSettings.DebugMode;
        _toggleDebugMode.onValueChanged.AddListener(ToggleDebugModeOnValueChanged);

        _toggleDNFShaking.isOn = OptionSettings.DNFShaking;
        _toggleDNFShaking.onValueChanged.AddListener(ToggleDNFShakingOnValueChanged);

        _toggleDamageValueEffect.isOn = OptionSettings.DamageValueEffect;
        _toggleDamageValueEffect.onValueChanged.AddListener(ToggleDamageValueEffectOnValueChanged);

        _textRollingSpeed.text = string.Format("{0}", OptionSettings.HealthRollingSpeed);
        _sliderRollingSpeed.value = OptionSettings.HealthRollingSpeed;
        _sliderRollingSpeed.onValueChanged.AddListener(SliderRollingSpeedOnValueChanged);
    }
    #endregion

    #region Toggle Handlings
    private void ToggleDebugModeOnValueChanged(bool value) {
        OptionSettings.DebugMode = value;

        Utility.OnDebugModeChanged();
    }

    private void ToggleDNFShakingOnValueChanged(bool value) {
        OptionSettings.DNFShaking = value;
    }

    private void ToggleDamageValueEffectOnValueChanged(bool value) {
        OptionSettings.DamageValueEffect = value;
    }

    private void SliderRollingSpeedOnValueChanged(float value) {
        int valueInt = Mathf.RoundToInt(value);

        _textRollingSpeed.text = string.Format("{0}", valueInt);
        OptionSettings.HealthRollingSpeed = valueInt;
    }
    #endregion
}
