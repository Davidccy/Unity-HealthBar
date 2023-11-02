using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    #region Serialized Fields
    [SerializeField] private Button _btnOnePunch = null;
    [SerializeField] private Button _btnTripleJab = null;

    [SerializeField] private float _skillCDOnePunch = 0;
    [SerializeField] private float _skillCDTripleJab = 0;

    [SerializeField] private Image _imageCDMaskOnePunch = null;
    [SerializeField] private Image _imageCDMaskTripleJab = null;
    #endregion

    #region Internal Fields
    private List<AttackInfo> _atkInfoList = new List<AttackInfo>();
    private float _cdOnePunch = 0;
    private float _cdTripleJab = 0;
    #endregion

    #region Mono Behaviour Hooks
    private void Awake() {
        _btnOnePunch.onClick.AddListener(ButtonOnePunchOnClick);
        _btnTripleJab.onClick.AddListener(ButtonTripleJabOnClick);
    }

    private void Update() {
        UpdateSkillCoolDown();
        UpdateSkillCoolDownMask();
        UpdateAttackInfo();
    }
    #endregion

    #region UI Button
    private void ButtonOnePunchOnClick() {
        int atkValue = 300 + Random.Range(-5, 5);
        _atkInfoList.Add(new AttackInfo() { Delay = 0, Value = atkValue });
    }

    private void ButtonTripleJabOnClick() {
        int firstAtkValue = 100 + Random.Range(-5, 5);
        _atkInfoList.Add(new AttackInfo() { Delay = 0, Value = firstAtkValue });

        int secondAtkValue = 200 + Random.Range(-5, 5);
        _atkInfoList.Add(new AttackInfo() { Delay = 0.1f, Value = secondAtkValue });

        int thirdAtkValue = 300 + Random.Range(-5, 5);
        _atkInfoList.Add(new AttackInfo() { Delay = 0.2f, Value = thirdAtkValue });
    }
    #endregion

    #region Internal Methods
    private void UpdateSkillCoolDown() {
        _cdOnePunch -= Time.deltaTime;
        _cdTripleJab -= Time.deltaTime;
    }

    private void UpdateSkillCoolDownMask() {
        //_imageCDMaskOnePunch.fillAmount = _skillCDOnePunch == 0 ? 1 : (1 - _cdOnePunch / _skillCDOnePunch);
        //_imageCDMaskTripleJab.fillAmount = _skillCDTripleJab == 0 ? 1 : (1 - _cdTripleJab / _skillCDTripleJab);
    }

    private void UpdateAttackInfo() {
        for (int i = _atkInfoList.Count - 1; i >= 0; i--) {
            if (_atkInfoList[i].Delay <= 0) {
                AttackHandler.AttackEnemy(_atkInfoList[i]);

                _atkInfoList.RemoveAt(i);
            }
            else {
                _atkInfoList[i].Delay -= Time.deltaTime;
            }
        }
    }
    #endregion
}
