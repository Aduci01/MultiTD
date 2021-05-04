using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour {
    public short id;
    public string playerName;
    [SerializeField] TMP_Text nameText;

    public bool isLocal;

    public RaceData raceData;

    public List<Building> buildings;
    public List<Unit> units;

    public short health;

    private void Start() {
        groundSpawner.Init(isLocal);

        buildings = new List<Building>();
        units = new List<Unit>();

        if (isLocal) {
            GameUi._instance.SetBottomUi(raceData);
        } else {

        }

        castle = Instantiate(raceData.castle, transform);
        castle.SetFlagColor(playerColor);

        portal = Instantiate(portalPrefab, transform);

        GameUi._instance.AddPlayerStatus(this);
    }

    public void Initialize(short id, string name, string raceId) {
        this.id = id;
        raceData = DataCollection.raceDb[raceId];

        playerName = name;
        nameText.text = playerName;

        playerColor = GameManager._instance.colors[id];
    }


    public void SetHp(short hp) {
        health = hp;

        GameUi._instance.SetHp(id, hp);
    }

    public void NewRound() {
        ResetAllEntities();

        castle.ResetMercenaries();
    }

    public Entity SearchForEntity(int serverId) {
        foreach (Building b in buildings) {
            if (b.serverId == serverId) return b;
        }

        foreach (Unit u in units) {
            if (u.serverId == serverId) return u;
        }

        return null;
    }

    #region Initial Fields
    [HideInInspector] public Castle castle;

    public Portal portalPrefab;
    [HideInInspector] public Portal portal;

    public Color playerColor;

    public GroundSpawner groundSpawner;
    #endregion

    #region Currencies
    public int goldCurrency, manaCurrency;
    public int goldIncomePerWave, manaIncomePerWave; //After each wave

    /// <summary>
    /// Setting the gold currency, Comes from server
    /// </summary>
    /// <param name="amount"></param>
    public void SetGoldCurrency(int amount) {
        goldCurrency = amount;

        GameUi._instance.goldText.text = amount + "";
    }

    /// <summary>
    /// Setting the mana currency, Comes from server
    /// </summary>
    /// <param name="amount"></param>
    public void SetManaCurrency(int amount) {
        manaCurrency = amount;

        GameUi._instance.manaText.text = amount + "";
    }

    #endregion

    #region Building
    /// <summary>
    /// After a validated placement request, Server tells the clients to spawn the building
    /// </summary>
    /// <param name="sId"></param>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    public void BuildBuilding(short sId, string id, Vector3 pos) {
        BuildingData bd = DataCollection.buildingDb[id];

        var building = Instantiate(bd.prefab, transform).GetComponent<Building>();
        building.transform.localPosition = pos;
        building.Init(true, sId, bd, this);

        buildings.Add(building);
        GameUi._instance.SetIncomeTexts();

        ParticleManager._instance.SpawnParticle(ParticleManager._instance.entityPlaced, building.transform, building.transform.position, Vector3.zero);
    }


    /// <summary>
    /// After a validated placement request, Server tells the clients to spawn the Unit
    /// </summary>
    /// <param name="sId"></param>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    public void BuildUnit(short sId, string id, Vector3 pos) {
        UnitData ud = DataCollection.unitDb[id];

        var unit = Instantiate(ud.prefab, transform).GetComponent<Unit>();
        unit.transform.localPosition = pos;
        unit.Init(true, sId, ud, this);

        units.Add(unit);
        GameUi._instance.SetIncomeTexts();

        ParticleManager._instance.SpawnParticle(ParticleManager._instance.unitPlaced, unit.transform, unit.transform.position, Vector3.left * 90);
    }

    public void ResetAllEntities() {
        foreach (Building b in buildings) {
            b.SetHealth(b.maxHealth);
            b.isDead = false;
            b.EnableEntity();

            b.gameObject.SetActive(true);
        }

        foreach (Unit u in units) {
            u.SetHealth(u.maxHealth);
            u.isDead = false;
            u.transform.position = u.startPos;
            u.EnableEntity();

            u.gameObject.SetActive(true);
        }
    }

    public void RemoveEntity(short serverId) {
        Entity e = SearchForEntity(serverId);
        if (e == null) return;

        if (e.GetComponent<Unit>() != null) {
            Unit u = e.GetComponent<Unit>();
            units.Remove(u);

            goldIncomePerWave -= u.data.stats.levels[u.level].goldIncome;
            manaIncomePerWave -= u.data.stats.levels[u.level].manaIncome;

            Destroy(u.gameObject);
        }

        if (e.GetComponent<Building>() != null) {
            Building b = e.GetComponent<Building>();
            buildings.Remove(b);

            goldIncomePerWave -= b.data.stats.levels[b.level].goldIncome;
            manaIncomePerWave -= b.data.stats.levels[b.level].manaIncome;

            Destroy(b.gameObject);
        }

        if (isLocal)
            GameUi._instance.SetIncomeTexts();

        if (GameManager._instance.allEntities.ContainsKey(serverId))
            GameManager._instance.allEntities.Remove(serverId);

        ParticleManager._instance.SpawnParticle(ParticleManager._instance.sell, null, e.transform.position, Vector3.zero);
    }

    #endregion

    #region Enemy Handling
    [Space(15)]
    [Header("Enemies")]
    public List<Enemy> enemies;

    public Transform[] spawnTransforms;
    int spawnId;

    public void SpawnEnemy(EnemyData e, short serverId, float posX = -9999, float posZ = -9999) {
        Enemy enemy = Instantiate(e.prefab, transform).GetComponent<Enemy>();
        enemy.Init(serverId, e, this);

        enemies.Add(enemy);

        if (posX == -9999)
            enemy.transform.position = spawnTransforms[spawnId++ % spawnTransforms.Length].position;
        else enemy.transform.position = new Vector3(posX, spawnTransforms[spawnId % spawnTransforms.Length].position.y, posZ);
    }

    public void AddMercenary(string sId) {
        EnemyData enemy = DataCollection.enemyDb[sId];
        castle.AddMercenary(enemy);

        goldIncomePerWave += enemy.stats.levels[0].goldIncome;

        if (isLocal) {
            GameUi._instance.SetIncomeTexts();
            MercenaryManager._instance.AddLimitUi(1);
        }
    }
    #endregion
}
