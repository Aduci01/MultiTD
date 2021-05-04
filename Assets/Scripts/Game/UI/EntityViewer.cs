using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityViewer : MonoBehaviour {
    public static EntityViewer _instance;

    public GameObject window;

    public Text nameText, descText, levelText;
    public Text costText;
    public Image icon;

    public Text damageTypeText;
    public Text atkSpeed, damage, health, range, armor, magicResist;
    public Text manaProductionText, goldProductionText;
    public Text healText;

    Entity currentEntity;
    Stats currentStat, nextStat;
    DamageType damageType;

    public GameObject sellButton;

    private void Awake() {
        _instance = this;
    }

    private void Update() {
        if (!Tools.IsPointerOverUIObject() && Input.GetMouseButtonDown(0))
            window.SetActive(false);
    }

    public void SetViewer(Enemy e) {
        nameText.text = e.data.displayName;
        descText.text = e.data.description;
        levelText.text = "Level " + (e.level + 1);

        icon.sprite = e.data.icon;

        currentEntity = e;
        currentStat = e.data.stats.levels[e.level];

        if (e.data.stats.levels.Length > e.level + 1)
            nextStat = e.data.stats.levels[e.level + 1];
        else nextStat = null;
        damageType = e.data.stats.damageType;

        SetStats();
        SetCost(e.data.stats.levels);

        sellButton.SetActive(false);

        window.SetActive(true);
    }

    public void SetViewer(Building b) {
        nameText.text = b.data.displayName;
        descText.text = b.data.description;
        levelText.text = "Level " + (b.level + 1);

        icon.sprite = b.data.icon;


        currentEntity = b;
        currentStat = b.data.stats.levels[b.level];

        if (b.data.stats.levels.Length > b.level + 1)
            nextStat = b.data.stats.levels[b.level + 1];
        else nextStat = null;
        damageType = b.data.stats.damageType;

        SetStats();
        SetCost(b.data.stats.levels);

        sellButton.SetActive(currentEntity.GetOwner() == GameManager._instance.localPlayer);

        window.SetActive(true);
    }

    public void SetViewer(Unit u) {
        nameText.text = u.data.displayName;
        descText.text = u.data.description;
        levelText.text = "Level " + (u.level + 1);

        icon.sprite = u.data.icon;

        currentEntity = u;
        currentStat = u.data.stats.levels[u.level];

        if (u.data.stats.levels.Length > u.level + 1)
            nextStat = u.data.stats.levels[u.level + 1];
        else nextStat = null;
        damageType = u.data.stats.damageType;

        SetStats();
        SetCost(u.data.stats.levels);

        sellButton.SetActive(currentEntity.GetOwner() == GameManager._instance.localPlayer);

        window.SetActive(true);
    }

    public void OnClickUpgrade() {
        ClientSend.UpgradeRequest(currentEntity.serverId);
    }

    public void OnEnterUpgradeButton() {
        SetStats(true);
    }

    public void OnExitUpgradeButton() {
        SetStats(false);
    }

    public void OnClickSell() {
        window.SetActive(false);
        currentEntity.SellEntityRequest();
    }

    void SetStats(bool showUpgrades = false) {
        if (currentStat.atkSpeed == 0)
            atkSpeed.transform.parent.gameObject.SetActive(false);
        else {
            atkSpeed.transform.parent.gameObject.SetActive(true);
            atkSpeed.text = currentStat.atkSpeed + "s";

            if (showUpgrades)
                atkSpeed.text += " → " + nextStat.atkSpeed + "s";
        }

        if (currentStat.damage == 0) {
            damage.transform.parent.gameObject.SetActive(false);
        } else {
            damage.text = currentStat.damage + "";
            damageTypeText.text = damageType.ToString() + " Damage";

            if (showUpgrades)
                damage.text += " → " + nextStat.damage;

            damage.transform.parent.gameObject.SetActive(true);
        }

        health.text = currentStat.hp + "";
        if (showUpgrades)
            health.text += " → " + nextStat.hp;

        if (currentStat.range == 0)
            range.transform.parent.gameObject.SetActive(false);
        else {
            range.text = currentStat.range * 100 + "";
            range.transform.parent.gameObject.SetActive(true);

            if (showUpgrades)
                range.text += " → " + nextStat.range * 100;
        }

        if (currentStat.goldIncome == 0)
            goldProductionText.transform.parent.gameObject.SetActive(false);
        else {
            goldProductionText.text = currentStat.goldIncome + " /wave";
            goldProductionText.transform.parent.gameObject.SetActive(true);

            if (showUpgrades)
                goldProductionText.text += " → " + nextStat.goldIncome;
        }

        if (currentStat.manaIncome == 0)
            manaProductionText.transform.parent.gameObject.SetActive(false);
        else {
            manaProductionText.text = currentStat.manaIncome + " /wave";
            manaProductionText.transform.parent.gameObject.SetActive(true);

            if (showUpgrades)
                range.text += " → " + nextStat.manaIncome;
        }

        armor.text = currentStat.armor + "%";
        if (showUpgrades)
            armor.text += " → " + nextStat.armor + "%";

        magicResist.text = currentStat.magicResist + "%";
        if (showUpgrades)
            magicResist.text += " → " + nextStat.magicResist + "%";

        if (currentStat.healAmount == 0)
            healText.transform.parent.gameObject.SetActive(false);
        else {
            healText.text = "" + currentStat.healAmount;
            healText.transform.parent.gameObject.SetActive(true);

            if (showUpgrades)
                healText.text += " → " + nextStat.healAmount;
        }
    }

    void SetCost(Stats[] stats) {
        PlayerManager owner = currentEntity.GetOwner();
        if (currentEntity.level >= stats.Length - 1 || GameManager._instance.localPlayer != owner)
            costText.transform.parent.gameObject.SetActive(false);
        else {
            int cost = stats[currentEntity.level + 1].price;
            costText.transform.parent.gameObject.SetActive(true);
            costText.text = cost + "";

            if (cost > owner.goldCurrency) {
                costText.color = Color.red;
            } else costText.color = Color.green;
        }
    }
}
