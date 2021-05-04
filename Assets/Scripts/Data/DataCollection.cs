using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;

/// <summary>
/// Static collection of all data fetched from the Playfab database
/// </summary>
public static class DataCollection {

    #region Catalogs & databases built from Catalogs
    public static Dictionary<string, CatalogItem> raceCatalog, buildingCatalog, unitCatalog, enemyCatalog;

    public static Dictionary<string, RaceData> raceDb = new Dictionary<string, RaceData>();
    public static Dictionary<string, BuildingData> buildingDb = new Dictionary<string, BuildingData>();
    public static Dictionary<string, UnitData> unitDb = new Dictionary<string, UnitData>();
    public static Dictionary<string, EnemyData> enemyDb = new Dictionary<string, EnemyData>();

    public static WaveParent waves;

    public static void GetData() {
        if (raceCatalog != null) return;

        raceCatalog = new Dictionary<string, CatalogItem>();
        buildingCatalog = new Dictionary<string, CatalogItem>();
        unitCatalog = new Dictionary<string, CatalogItem>();
        enemyCatalog = new Dictionary<string, CatalogItem>();

        raceDb = new Dictionary<string, RaceData>();
        buildingDb = new Dictionary<string, BuildingData>();
        unitDb = new Dictionary<string, UnitData>();
        enemyDb = new Dictionary<string, EnemyData>();

        GetCatalogs();
    }

    static void GetCatalogs() {
        GetCatalogItemsRequest request = new GetCatalogItemsRequest {
            CatalogVersion = "Races"
        };
        PlayFabClientAPI.GetCatalogItems(request, OnCatalogResultRace, error => { Debug.LogError(error.Error); });
    }

    static void OnCatalogResultRace(GetCatalogItemsResult result) {
        for (int i = 0; i < result.Catalog.Count; i++) {
            AddItemToDB(result.Catalog[i].ItemId, result.Catalog[i]);
        }

        GetCatalogItemsRequest request2 = new GetCatalogItemsRequest {
            CatalogVersion = "Buildings"
        };
        PlayFabClientAPI.GetCatalogItems(request2, OnCatalogResultOther, error => { Debug.LogError(error.Error); });

        GetCatalogItemsRequest request3 = new GetCatalogItemsRequest {
            CatalogVersion = "Units"
        };
        PlayFabClientAPI.GetCatalogItems(request3, OnCatalogResultOther, error => { Debug.LogError(error.Error); });

        GetCatalogItemsRequest request4 = new GetCatalogItemsRequest {
            CatalogVersion = "Enemies"
        };
        PlayFabClientAPI.GetCatalogItems(request4, OnCatalogResultOther, error => { Debug.LogError(error.Error); });

        LoadingScreen._instance.doneProgress++;
    }

    static void OnCatalogResultOther(GetCatalogItemsResult result) {
        for (int i = 0; i < result.Catalog.Count; i++) {
            AddItemToDB(result.Catalog[i].ItemId, result.Catalog[i]);
        }
    }

    static void AddItemToDB(string id, CatalogItem ci) {
        switch (ci.ItemClass) {
            case "Race":
                raceCatalog.Add(id, ci);

                AddRace(id, ci);
                break;
            case "Building":
                buildingCatalog.Add(id, ci);

                AddBuilding(id, ci);
                break;
            case "Unit":
                unitCatalog.Add(id, ci);

                AddUnit(id, ci);
                break;
            case "Enemy":
                enemyCatalog.Add(id, ci);

                AddEnemy(id, ci);
                break;
        }
    }

    static void AddRace(string id, CatalogItem ci) {
        RaceData newRace = new RaceData(id, ci);

        raceDb.Add(id, newRace);
    }

    static void AddBuilding(string id, CatalogItem ci) {
        BuildingData newBuilding = new BuildingData(id, ci);
        buildingDb.Add(id, newBuilding);

        raceDb[newBuilding.stats.raceId].buildings.Add(newBuilding);
    }

    static void AddUnit(string id, CatalogItem ci) {
        UnitData newUnit = new UnitData(id, ci);
        unitDb.Add(id, newUnit);

        if (raceDb.ContainsKey(newUnit.stats.raceId))
            raceDb[newUnit.stats.raceId].units.Add(newUnit);
    }

    static void AddEnemy(string id, CatalogItem ci) {
        EnemyData newEnemy = new EnemyData(id, ci);
        enemyDb.Add(id, newEnemy);
    }
    #endregion
}

public enum DamageType { Magic, Physical, Pure };

[System.Serializable]
public class Stats {
    public ushort price;

    public ushort hp;
    public float movementSpeed;

    public ushort damage;
    public float atkSpeed;

    public float range;

    //Defense
    public int magicResist = 0;
    public int armor = 0;

    //Special values
    public float splashRadius = 0;
    public int slowMovement = 0;

    public int healAmount = 0;
    public int healRate = 10;

    public float summonRate = 0f;
    public string summonId;

    //Income Modifiers
    public int goldIncome = 0;
    public int manaIncome = 0;
}

[System.Serializable]
public class WaveParent {
    public WaveData[] waves;
}

[System.Serializable]
public class WaveData {
    public int waveId;
    public WaveType[] wavePart;
}

[System.Serializable]
public class WaveType {
    public string enemyType;
    public int amount;
}