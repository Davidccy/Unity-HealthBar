using System.Collections.Generic;
using UnityEngine;

public static class UIHealthHandler {
    #region Internal Fields
    private static List<UIHealthBase> _uiHealthList = new List<UIHealthBase>();
    #endregion

    #region APIs
    public static void RegisterUIHealth(UIHealthBase uiHealth) {
        _uiHealthList.Add(uiHealth);
    }
    #endregion

    #region APIs
    public static void EnemyHealthChanged(bool isInit, int oldValue, int newValue, int maxValue) {
        if (_uiHealthList == null) {
            return;
        }

        for (int i = 0; i < _uiHealthList.Count; i++) {
            _uiHealthList[i].OnValueUpdated(isInit, oldValue, newValue, maxValue);
        }
    }
    #endregion
}
