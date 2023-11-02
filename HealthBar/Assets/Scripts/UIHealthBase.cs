using UnityEngine;

public class UIHealthBase : MonoBehaviour {
    // NOTE:
    // Fill Value / Value per Line = Fill Amount

    #region Serialized Fields
    [SerializeField] protected int _valuePerLine = 1000;
    #endregion

    #region Internal Fields
    protected int _maxHealth;
    protected int _curHealth;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        UIHealthHandler.RegisterUIHealth(this);

        Utility.RegisterDebugModeCallback(this, (bool b) => OnDebugModeChanged());

        OnAwake();
    }
    #endregion

    #region Virtual Methods
    public virtual void OnAwake() { 

    }

    public virtual void OnDebugModeChanged() { 

    }

    public virtual void OnValueUpdated(bool isInit, int oldValue, int newValue, int maxValue) { 

    }
    #endregion

    #region Protected Methods
    protected int GetLineIndex(float value) {
        int valueInt = Mathf.CeilToInt(value);

        int index = valueInt / _valuePerLine;
        if (valueInt >= _valuePerLine && valueInt % _valuePerLine == 0) {
            index -= 1;
        }

        return index;
    }

    protected int GetMaxValueInLine(int lineIndex) {
        return (lineIndex + 1) * _valuePerLine;
    }

    protected int GetMinValueInLine(int lineIndex) {
        return lineIndex * _valuePerLine;
    }
    #endregion
}
