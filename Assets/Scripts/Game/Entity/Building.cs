using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity {
    public BuildingData data;

    public Placeable placeable;
    PlayerManager pm;

    public Shooter shooter;
    public HealBehaviour healer;

    [SerializeField] UpgradeObject[] upgradeLevels;

    public void Init(bool isPlaced, short serverId, BuildingData data, PlayerManager pm = null) {
        base.Init(0);
        OnDeathEvent += OnDestroyed;

        this.data = data;

        if (isPlaced) {
            SetId(serverId);

            placeable.enabled = false;
            placeable.validatorSprite.gameObject.SetActive(false);

            this.pm = pm;

            SetStats();

            EnableEntity();
        } else {
            if (radiusIndicator != null)
                radiusIndicator.gameObject.SetActive(true);

            DisableScripts();
        }
    }

    public override void SetStats() {
        Stats stats = data.stats.levels[level];

        if (shooter != null) {
            shooter.stats = stats;
            shooter.partToRotate = upgradeLevels[level].partToRotate;

            if (upgradeLevels[level].firePoint != null)
                shooter.firePoint = upgradeLevels[level].firePoint;


            radiusIndicator.transform.localScale = Vector3.one * stats.range * 2;

            shooter.damageType = data.stats.damageType;
        }

        if (healer != null) {
            healer.healAmount = stats.healAmount;
            healer.healRate = stats.healRate;

            healer.pm = pm;
        }

        maxHealth = health = stats.hp;


        //Setting income modifiers
        pm.goldIncomePerWave += stats.goldIncome;
        pm.manaIncomePerWave += stats.manaIncome;
        if (level > 0) {
            pm.goldIncomePerWave -= data.stats.levels[level - 1].goldIncome;
            pm.manaIncomePerWave -= data.stats.levels[level - 1].manaIncome;
        }

        GameUi._instance.SetIncomeTexts();

        SetCurrentStats(stats);

        //Special values
        Cauldron c = GetComponent<Cauldron>();
        if (c != null) c.SetValues();
    }

    // Use this for initialization
    void Start() {
        if (shooter != null)
            InvokeRepeating("UpdateTarget", 0f, 1f);
    }

    void UpdateTarget() {
        if (GameManager._instance.state != GameManager.GameState.InWave) return;

        float shortestDistance = Mathf.Infinity;
        Entity nearestE = null;

        foreach (Enemy e in pm.enemies) {
            if (e.isDead) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, e.transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestE = e;
            }
        }

        if (nearestE != null) {
            shooter.target = nearestE;
        } else {
            shooter.target = null;
        }
    }

    public override void OnDestroyed() {
        base.OnDestroyed();
    }

    public override void AddScriptsToDisabledList() {
        scriptsToDisableAfterDeath.Add(shooter);
        scriptsToDisableAfterDeath.Add(healer);
    }

    public override void Upgrade() {
        if (level >= data.stats.levels.Length - 1) return;

        level++;
        SetStats();

        upgradeLevels[level - 1].gameObject.SetActive(false);
        upgradeLevels[level].gameObject.SetActive(true);

        ParticleManager._instance.SpawnParticle(ParticleManager._instance.entityPlaced, transform, transform.position, Vector3.zero);
    }

    public override void SetViewer() {
        EntityViewer._instance.SetViewer(this);
    }

    public override PlayerManager GetOwner() {
        return pm;
    }

    public override void SetTarget(short id, bool nullTarget) {
        Entity e = GameManager._instance.allEntities[id];
        shooter.target = e;
    }

    public override void AddSlow(int value, float time) {

    }
}
