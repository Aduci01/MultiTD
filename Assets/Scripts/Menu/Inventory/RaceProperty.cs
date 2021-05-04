using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceProperty : MonoBehaviour {
    public Image icon;

    public UnitData unitData;
    public BuildingData buildingData;

    public void SetUi(UnitData ud) {
        unitData = ud;
        icon.sprite = ud.icon;

        gameObject.SetActive(true);
    }

    public void SetUi(BuildingData bd) {
        buildingData = bd;
        icon.sprite = bd.icon;

        gameObject.SetActive(true);
    }
}
