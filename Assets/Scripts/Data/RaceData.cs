using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

[System.Serializable]
public class RaceData {
    public string displayName;
    public string description;

    public string id;

    public Sprite icon;

    public RaceJson stats;

    public Castle castle;
    public List<BuildingData> buildings;
    public List<UnitData> units;

    public RaceData(string id, CatalogItem ci) {
        this.id = id;

        buildings = new List<BuildingData>();
        units = new List<UnitData>();

        displayName = ci.DisplayName;
        description = ci.Description;

        stats = JsonUtility.FromJson<RaceJson>(ci.CustomData);

        icon = Resources.Load<Sprite>("Races/" + id + "/" + id);
        castle = Resources.Load<Castle>("Races/" + id + "/" + "Building/" + "Castle");
    }
}

[System.Serializable]
public class RaceJson {

}
