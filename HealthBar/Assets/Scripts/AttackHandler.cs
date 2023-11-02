using UnityEngine;

public class AttackInfo {
    public float Delay;
    public int Value;
}

public static class AttackHandler {
    private static Enemy _targetEnemy = null;

    public static void RegisterEnemy(Enemy e) {
        _targetEnemy = e;
    }

    public static void AttackEnemy(AttackInfo atk) {
        if (_targetEnemy == null) {
            return;
        }

        _targetEnemy.OnDamaged(atk.Value);
    }
}
