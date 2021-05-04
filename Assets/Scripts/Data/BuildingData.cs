using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

[System.Serializable]
public class BuildingData {
    public string displayName;
    public string description;

    public string id;

    public Sprite icon;
    public GameObject prefab;

    public BuildingJson stats;


    public BuildingData(string id, CatalogItem ci) {
        this.id = id;

        displayName = ci.DisplayName;
        description = ci.Description;

        stats = JsonUtility.FromJson<BuildingJson>(ci.CustomData);

        icon = Resources.Load<Sprite>("Races/" + stats.raceId + "/" + id);
        prefab = Resources.Load<GameObject>("Races/" + stats.raceId + "/" + ci.ItemClass + "/" + id);
    }
}

[System.Serializable]
public class BuildingJson {
    public string raceId;

    public DamageType damageType = DamageType.Physical;
    public Stats[] levels;
}

