using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Entity {
    public UnitData data;
    PlayerManager pm;

    public Placeable placeable;
    public Shooter shooter;
    public MovementBehaviour movement;
    public HealBehaviour healer;

    [HideInInspector] public Vector3 startPos;

    public void Init(bool isPlaced, short serverId, UnitData data, PlayerManager pm = null) {
        base.Init(1.0f);
        OnDeathEvent += OnDestroyed;

        this.data = data;

        animator.Play("Idle");

        if (isPlaced) {
            SetId(serverId);

            placeable.enabled = false;
            placeable.validatorSprite.gameObject.SetActive(false);

            this.pm = pm;

            SetStats();

            startPos = transform.position;

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

            shooter.damageType = data.stats.damageType;

            if (radiusIndicator != null)
                radiusIndicator.transform.localScale = Vector3.one * stats.range * 2;
        }

        if (healer != null) {
            healer.healAmount = stats.healAmount;
            healer.healRate = stats.healRate;

            healer.pm = pm;
        }

        maxHealth = health = stats.hp;


        movement.speed = stats.movementSpeed;

        SetCurrentStats(stats);
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
            movement.target = nearestE.transform;
            shooter.target = nearestE;
        } else {
            movement.target = null;
            shooter.target = null;
        }
    }

    private void Update() {
        if (shooter.isShooting) {
            movement.canMove = false;
        } else {
            movement.canMove = true;

            if (movement.target != null) {
                animator.Play("Run");
            } else {
                animator.Play("Idle");
            }
        }
    }

    public override void AddScriptsToDisabledList() {
        scriptsToDisableAfterDeath.Add(shooter);
        scriptsToDisableAfterDeath.Add(movement);
        scriptsToDisableAfterDeath.Add(this);
        scriptsToDisableAfterDeath.Add(healer);
    }

    public override void Upgrade() {
        if (level >= data.stats.levels.Length - 1) return;

        level++;
        SetStats();

        ParticleManager._instance.SpawnParticle(ParticleManager._instance.unitPlaced, transform, transform.position, Vector3.left * 90);
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
        movement.target = e.transform;
    }

    public override void AddSlow(int value, float time) {
        movement.AddSlow(value, time);
    }
}
