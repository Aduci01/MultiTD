using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatus : MonoBehaviour {
    public Image hpFillImage;
    public TMP_Text nameText, hpText;

    public Image iconImage;

    int playerId;

    public void InitUi(string pName, RaceData rd, Color c, int id) {
        playerId = id;

        nameText.text = pName;
        iconImage.sprite = rd.icon;

        SetHp(100);
    }

    public void SetHp(int hp) {
        hpFillImage.fillAmount = hp / 100f;
        hpText.text = hp + "";
    }

    public void OnClick() {
        RtsCamera._instance.SetPositionToPlayer(playerId);
    }
}
