using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MercenaryUiEntity : MonoBehaviour {
    public EnemyData data;

    public Image icon;
    public TMP_Text description, cost, goldIncome;
    public TMP_Text nameText;

    public GameObject infoPanel;
    public Transform infoWindowParent;

    /// <summary>
    /// Setting up UI data
    /// </summary>
    /// <param name="ed"></param>
    public void SetUp(EnemyData ed) {
        icon.sprite = ed.icon;

        description.text = ed.description;
        nameText.text = ed.displayName;
        cost.text = ed.stats.levels[0].price + "";

        if (goldIncome != null)
            goldIncome.text = "+" + ed.stats.levels[0].goldIncome + " /wave";

        data = ed;

        infoPanel.transform.parent = infoWindowParent;
    }

    /// <summary>
    /// Clicking on mercenary ui -> request to server
    /// </summary>
    public void OnClick() {
        if (GameManager._instance.state != GameManager.GameState.PreWave) {
            MessageHandler._instance.ShowMessage("You can only buy mercenaries during Build Phase", 1f, Color.red);
        }

        if (MercenaryManager._instance.IsLimitReached()) {
            MessageHandler._instance.ShowMessage("You cannot buy more mercenaries in this round", 1f, Color.red);
        }

        ClientSend.MercenaryRequest(data);
    }

    /// <summary>
    /// If pointer is above UI show Info panel
    /// </summary>
    public void OnPointerEnter() {
        if (GameManager._instance.localPlayer.manaCurrency >= data.stats.levels[0].price) {
            cost.color = Color.green;
        } else cost.color = Color.red;

        infoPanel.SetActive(true);
    }
}
