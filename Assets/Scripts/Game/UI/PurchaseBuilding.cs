using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseBuilding : PurchasableIcon {
    public BuildingData data;
    public InfoPopup popup;

    public override void OnClick() {
        if (GameManager._instance.localPlayer.goldCurrency < data.stats.levels[0].price) {
            MessageHandler._instance.ShowMessage("You don't have enough gold", 1f, Color.red);
            return;
        }

        if (GameManager._instance.state != GameManager.GameState.PreWave) {
            MessageHandler._instance.ShowMessage("You can only build during Build Phase", 1f, Color.red);
            return;
        }

        var building = InstatiateObject(data.prefab, GameManager._instance.localPlayer, data.prefab.GetComponent<Placeable>().offset).GetComponent<Building>();
        building.Init(false, 0, data);
    }

    public void Init(BuildingData bd) {
        data = bd;
        icon.sprite = data.icon;

        gameObject.SetActive(true);

        popup.Init(data.displayName, data.description, data.stats.levels[0].price);
    }

    public override void PointerEnter() {
        EntityViewer._instance.SetViewer(data.prefab.GetComponent<Building>());
    }

    public override void PointerExit() {
        EntityViewer._instance.window.SetActive(false);
    }
}
