using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealBehaviour : MonoBehaviour {
    [HideInInspector] public PlayerManager pm;

    public int healAmount;
    public float healRate;
    float healTimer;

    Entity target;

    public GameObject healEffect, healAura;

    // Start is called before the first frame update
    void Start() {
        InvokeRepeating("UpdateTarget", 0.5f, 1f);
    }

    void UpdateTarget() {
        if (pm == null) return;

        float hpRate = 1;
        Entity lowestHpEntity = null;

        foreach (Building b in pm.buildings) {
            if (b.isDead) continue;

            float percent = b.health / b.maxHealth;
            if (percent < hpRate) {
                hpRate = percent;
                lowestHpEntity = b;
            }
        }

        foreach (Unit u in pm.units) {
            if (u.isDead) continue;

            float percent = u.health / u.maxHealth;
            if (percent < hpRate) {
                hpRate = percent;
                lowestHpEntity = u;
            }
        }

        target = lowestHpEntity;
    }

    // Update is called once per frame
    void Update() {
        if (target == null) return;

        if (healTimer <= 0f) {
            Heal();

            healTimer = healRate;
        }

        healTimer -= Time.deltaTime;
    }

    void Heal() {
        target.TakeDamage(-healAmount, DamageType.Pure);

        if (healEffect != null && RtsCamera._instance.IsVisible(target.transform.position)) {
            var s = Instantiate(healEffect);
            s.transform.position = target.transform.position;
        }

        if (healAura != null && RtsCamera._instance.IsVisible(transform.position)) {
            healAura.SetActive(true);
        }
    }
}
