using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Building type, which converts mana into gold
/// </summary>
public class Cauldron : MonoBehaviour {
    public Building building;
    PlayerManager pm;

    public int manaConverted; //Mana already converted
    public int maxManaConverted; //The maximum mana which can be converted
    public float convertRatio; //The convertion ratio (ex: 2:1)

    public int amount; //the amount of mana which will be converted into gold

    // Start is called before the first frame update
    void Start() {
        building = GetComponent<Building>();
        pm = building.GetOwner();

        SetValues();
    }

    /// <summary>
    /// Setting values after spawn and each upgrade
    /// </summary>
    public void SetValues() {
        maxManaConverted = GetMaxMana();
        convertRatio = GetRatio();
    }

    int GetMaxMana() {
        switch (building.level) {
            case 0: return 70;
            case 1: return 165;
            case 2: return 300;
            default: return 70;
        }
    }

    float GetRatio() {
        switch (building.level) {
            case 0: return 2;
            case 1: return 1.5f;
            case 2: return 1.25f;
            default: return 2;
        }
    }

    private void OnMouseUpAsButton() {
        CauldronUi._instance.SetupWindow(this);
    }
}
