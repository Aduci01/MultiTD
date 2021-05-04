using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {
    [Space(15)]

    public EnemyData data;

    PlayerManager pm;

    public Shooter shooter;
    public MovementBehaviour movement;


    public void Init(short serverId, EnemyData data, PlayerManager pm) {
        base.Init(1.5f);
        AddScriptsToDisabledList();

        OnDeathEvent += OnDestroyed;

        this.data = data;
        SetId(serverId);

        this.pm = pm;

        SetStats();
    }

    public override void SetStats() {
        Stats stats = data.stats.levels[level];

        if (shooter != null) {
            shooter.damageType = data.stats.damageType;
            shooter.stats = stats;
        }

        movement.speed = stats.movementSpeed;

        maxHealth = health = stats.hp;

        SetCurrentStats(stats);

    }

    // Use this for initialization
    IEnumerator Start() {
        isDead = true;
        yield return new WaitForSeconds(0.65f);

        isDead = false;
        //InvokeRepeating("UpdateTarget", 0f, 1f);
    }

    void UpdateTarget() {
        float shortestDistance = Mathf.Infinity;
        Entity nearestE = null;

        foreach (Building b in pm.buildings) {
            if (b.isDead) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, b.transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestE = b;
            }
        }

        foreach (Unit u in pm.units) {
            if (u.isDead) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, u.transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestE = u;
            }
        }

        if (nearestE != null) {
            movement.target = nearestE.transform;
            shooter.target = nearestE;
        } else {
            movement.target = pm.castle.transform;
            shooter.target = null;
        }
    }

    private void Update() {
        if (shooter.isShooting) {
            movement.canMove = false;
        } else {
            movement.canMove = true;
        }

        if (movement.canMove && movement.target != null) {
            animator.Play("Run");
        }
    }

    public override void OnDestroyed() {
        base.OnDestroyed();

        pm.enemies.Remove(this);
        GameManager._instance.allEntities.Remove(serverId);

        Destroy(gameObject, timeToDestroy);

        if (Vector3.Distance(transform.position + Vector3.down * transform.position.y, pm.castle.transform.position + Vector3.down * pm.castle.transform.position.y) > 1.5f)
            ParticleManager._instance.SpawnParticle(ParticleManager._instance.goldGained, transform, transform.position + Vector3.up * 2, Vector3.right * 35f);
    }

    public override void AddScriptsToDisabledList() {
        scriptsToDisableAfterDeath.Add(shooter);
        scriptsToDisableAfterDeath.Add(movement);
        scriptsToDisableAfterDeath.Add(this);
    }

    public override void Upgrade() {

    }

    public override void SetViewer() {
        EntityViewer._instance.SetViewer(this);
    }

    public override PlayerManager GetOwner() {
        return pm;
    }

    public override void SetTarget(short id, bool nullTarget) {
        if (nullTarget) {
            shooter.target = null;
            movement.target = pm.castle.transform;
            return;
        }

        Entity e = GameManager._instance.allEntities[id];
        shooter.target = e;
        movement.target = e.transform;
    }

    public override void AddSlow(int value, float time) {
        movement.AddSlow(value, time);
    }
}
