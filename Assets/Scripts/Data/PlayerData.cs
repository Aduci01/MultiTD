

using UnityEngine;
/// <summary>
/// Static function for keeping the player data
/// </summary>
public static class PlayerData {

    public static string playFabId = "";
    public static string playerName = "Player";

    public static string selectedRaceId = "Humans";
    public static RaceData raceData;

    public static string playfabEntityID;
    public static string playfabEntityType;

    public static int PlayerLVL;
    public static int PlayerXP;

    public static int PlayerRank;
    public static int playerTrophies;

    /// <summary>
    /// Setting the race of the player and all the related ui elements
    /// </summary>
    /// <param name="rd"></param>
    public static void SetRace(RaceData rd) {
        selectedRaceId = rd.id;
        raceData = rd;

        UIManager._instance.SetRace(rd);

        PlayerPrefs.SetString("CURRENT_RACE", rd.id);
    }

    static int[] xpForNextLevelArray = { 20, 50, 100, 200, 400, 1000, 2000, 5000, 10000, 30000, 40000, 80000 };
    public static int GetXpForNextLevel(int lv) {
        return xpForNextLevelArray[lv - 1];
    }
    /*
    public static CardData currentClass;

    static int[] coinUpgradeCostArray = { 10, 20, 50, 150, 400, 1000, 2000, 4000, 8000, 20000, 50000, 100000, 200000, 300000 };
    public static int GetCoinCostForUpgrade(int lv, CardCollection.UnitRarity rarity) {
        if (rarity == CardCollection.UnitRarity.RARE) lv += 2;
        else if (rarity == CardCollection.UnitRarity.EPIC) lv += 4;
        else if (rarity == CardCollection.UnitRarity.LEGENDARY) lv += 5;

        return coinUpgradeCostArray[lv - 1];
    }

    static int[] stackAmountArray = { 2, 4, 10, 20, 50, 100, 200, 400, 800, 1000, 2000, 5000 };
    public static int GetStackAmountToNextLevel(int lv) {
        return stackAmountArray[lv - 1];
    }*/
}
