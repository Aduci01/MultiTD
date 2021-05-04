using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Entity : MonoBehaviour {
    [Header("Entity")]
    public static short entityId = Int16.MinValue + 1;
    [HideInInspector] public short serverId;

    public int level;

    public int health, maxHealth;
    public HealthBar hpBar;

    public Transform radiusIndicator;

    public Animator animator;

    public delegate void VoidDelegates();
    public event VoidDelegates OnDeathEvent;
    public bool isDead;

    private Stats currentStats;

    [HideInInspector] public List<MonoBehaviour> scriptsToDisableAfterDeath;
    [HideInInspector] public float timeToDestroy;

    public virtual void Init(float t) {
        timeToDestroy = t;
        scriptsToDisableAfterDeath = new List<MonoBehaviour>();
    }

    public void SetId(short id) {
        serverId = id;
        GameManager._instance.allEntities.Add(serverId, this);
    }

    /// <summary>
    /// Client side prediction
    /// </summary>
    /// <param name="dmg"></param>
    public void TakeDamage(int dmg, DamageType dt) {
        float modifier = 1;
        if (dt == DamageType.Magic) modifier = 1 - currentStats.magicResist / 100f;
        if (dt == DamageType.Physical) modifier = 1 - currentStats.armor / 100f;

        health -= (int)(dmg * modifier);

        hpBar.SetValue(health / (float)maxHealth);
    }

    public void SetHealth(int health) {
        this.health = health;
        hpBar.SetValue(health / (float)maxHealth);


        if (health <= 0) {
            OnDeathEvent?.Invoke();
        }
    }

    private void OnMouseEnter() {
        hpBar.gameObject.SetActive(true);

        if (radiusIndicator != null)
            radiusIndicator.gameObject.SetActive(true);
    }

    private void OnMouseExit() {
        hpBar.gameObject.SetActive(false);

        if (radiusIndicator != null)
            radiusIndicator.gameObject.SetActive(false);
    }

    private void OnMouseUpAsButton() {
        SetViewer();
    }

    public virtual void OnDestroyed() {
        isDead = true;

        Invoke("DeactivateObject", timeToDestroy);

        DisableScripts();

        if (animator != null)
            animator.Play("Die");

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            GetComponent<Collider>().enabled = false;
        }
    }

    public void DisableScripts() {
        foreach (MonoBehaviour m in scriptsToDisableAfterDeath) {
            m.enabled = false;
        }
    }

    public void EnableEntity() {
        foreach (MonoBehaviour m in scriptsToDisableAfterDeath) {
            m.enabled = true;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = false;
        }

        if (radiusIndicator != null)
            radiusIndicator.gameObject.SetActive(false);

        if (hpBar != null)
            hpBar.gameObject.SetActive(false);
    }

    void DeactivateObject() {
        gameObject.SetActive(false);
    }

    public void SetCurrentStats(Stats s) {
        currentStats = s;
    }

    public void SellEntityRequest() {
        ClientSend.SellEntityRequest(this);
    }

    public abstract void SetStats();

    public abstract void AddScriptsToDisabledList();

    public abstract void Upgrade();

    public abstract void SetViewer();

    public abstract PlayerManager GetOwner();

    public abstract void SetTarget(short id, bool nullTarget);

    public abstract void AddSlow(int value, float time);
}
