using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGame : MonoBehaviour {
    public static EndGame _instance;

    public GameObject window, win, defeat;
    public GameObject tick;
    public TMP_Text trophyText;

    public Image chest;

    GameResult gameResult;

    private void Awake() {
        _instance = this;
    }

    public void SetUpWindow(UserDataRecord data) {
        tick.SetActive(false);

        gameResult = JsonUtility.FromJson<GameResult>(data.Value);

        win.SetActive(gameResult.isWinner);
        defeat.SetActive(!gameResult.isWinner);

        trophyText.text = gameResult.gainedTrophies + "";

        if (gameResult.winReward == "") {
            chest.enabled = false;

            if (gameResult.isWinner)
                tick.SetActive(true);
        } else {
            chest.enabled = true;

            if (gameResult.winReward == "gold_chest") {
                chest.sprite = ChestOpening._instance.chestSettings.goldChestLocked;
                ChestOpening._instance.SetGoldChest();
            } else if (gameResult.winReward == "silver_chest") {
                chest.sprite = ChestOpening._instance.chestSettings.silverChestLocked;
                ChestOpening._instance.SetSilverChest();
            } else if (gameResult.winReward == "bronze_chest") {
                chest.sprite = ChestOpening._instance.chestSettings.bronzeChestLocked;
                ChestOpening._instance.SetBronzeChest();
            }
        }

        window.SetActive(true);
    }

    public void OnClickOk() {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest {
            FunctionName = "checkEndGame",
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnResult, error => { Debug.LogError(error.Error); window.SetActive(false); });
    }

    private void OnResult(ExecuteCloudScriptResult result) {
        window.SetActive(false);

        System.Object needObj, chestStringObj;

        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        if (jsonResult.TryGetValue("return", out needObj)) return;

        ChestOpening._instance.OnClaimResult(result);
    }
}

[System.Serializable]
public class GameResult {
    public bool isWinner;
    public string winReward = "";
    public int gainedTrophies;
}
