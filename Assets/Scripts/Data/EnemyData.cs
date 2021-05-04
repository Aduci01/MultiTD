using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

[System.Serializable]
public class EnemyData {
    public string displayName;
    public string description;

    public string id;

    public EnemyJson stats;

    public Sprite icon;
    public GameObject prefab;

    public EnemyData(string id, CatalogItem ci) {
        this.id = id;

        displayName = ci.DisplayName;
        description = ci.Description;

        stats = JsonUtility.FromJson<EnemyJson>(ci.CustomData);

        prefab = Resources.Load<GameObject>("Enemies/" + id);
        icon = Resources.Load<Sprite>("Enemies/" + id + "_icon");
    }
}

public class EnemyJson {
    public bool isPurchasable;
    public int hpConsume = 1;

    public DamageType damageType = DamageType.Physical;
    public Stats[] levels;
}
