using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RaceUi : MonoBehaviour {
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    public RaceData raceData;

    public void SetRace(RaceData rd) {
        raceData = rd;

        iconImage.sprite = rd.icon;
        nameText.text = rd.displayName;
        descriptionText.text = rd.description;

        SetupInspector(rd);
    }

    public void OnClick() {
        PlayerData.SetRace(raceData);
    }


    public RaceProperty unitPrefab, buildingPrefab;

    public List<RaceProperty> buildings, units;

    public void SetupInspector(RaceData rd) {
        foreach (UnitData u in rd.units) {
            var clone = Instantiate(unitPrefab, unitPrefab.transform.parent);
            clone.SetUi(u);

            units.Add(clone);
        }


        foreach (BuildingData b in rd.buildings) {
            var clone = Instantiate(buildingPrefab, buildingPrefab.transform.parent);
            clone.SetUi(b);

            buildings.Add(clone);
        }
    }
}
