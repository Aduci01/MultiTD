using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPopup : MonoBehaviour {
    public TMP_Text nameText, descriptionText, stat1, stat2;
    public TMP_Text costText;

    int cost;

    public void Init(string name, string desc, int cost, string s1 = "", string s2 = "") {
        nameText.text = name;
        descriptionText.text = desc;
        costText.text = cost + "";

        this.cost = cost;

        if (stat1 != null)
            stat1.text = s1;

        if (stat2 != null)
            stat2.text = s2;
    }

    public void OnPointerEnter() {
        if (GameManager._instance.localPlayer.goldCurrency >= cost) {
            costText.color = Color.green;
        } else costText.color = Color.red;
    }
}
