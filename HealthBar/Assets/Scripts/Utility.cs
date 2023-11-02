using System;
using System.Collections.Generic;

public static class Utility {
    #region Internal Fields
    private static Dictionary<object, Action<bool>> _debugAction = new Dictionary<object, Action<bool>>();
    #endregion

    #region APIs
    public static void OnDebugModeChanged() {
        foreach (KeyValuePair<object, Action<bool>> pair in _debugAction) {
            pair.Value(OptionSettings.DebugMode);
        }
    }

    public static void RegisterDebugModeCallback(object obj, Action<bool> action) {
        if (_debugAction.ContainsKey(obj)) {
            return;
        }
        _debugAction.Add(obj, action);
    }

    public static void UnregisterDebugModeCallback(object obj, Action<bool> action) {
        if (!_debugAction.ContainsKey(obj)) {
            return;
        }
        _debugAction.Remove(obj);
    }
    #endregion
}
