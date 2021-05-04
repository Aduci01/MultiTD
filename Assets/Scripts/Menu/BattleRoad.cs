using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class BattleRoad : MonoBehaviour {
    public static BattleRoad _instance;

    public static BattleRoadJson jsonData;

    public BattleRoadReward[] rewards;

    public TMP_Text winCounterText, timeLeftText;
    public Slider fillSlider;

    bool requestInProgress;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {
        jsonData = new BattleRoadJson();
        //collectButton.gameObject.SetActive(false);
    }

    public void SetData() {
        OnGetUserData();
    }

    /// <summary>
    /// Getting weeklyWin data from playfab
    /// </summary>
    /// <param name="result"></param>
    private void OnGetUserData() {
        if (PlayFabScript.userData.ContainsKey("battleRoad")) {
            jsonData = JsonUtility.FromJson<BattleRoadJson>(PlayFabScript.userData["battleRoad"].Value);

            CultureInfo c = new CultureInfo("en-US");
            DateTime result;

            if (DateTime.TryParse(jsonData.resetDate, out result)) {
                if (PlayFabScript.timeAtLogin != null && PlayFabScript.timeAtLogin.Time.Day != result.Day) {
                    ResetDailyData();
                } else {
                    SetVisuals();
                }
            }
        } else {
            ResetDailyData();
        }
    }

    /// <summary>
    /// Setting up visuals 
    /// </summary>
    void SetVisuals() {
        bool[] collected = new bool[3];
        collected[0] = jsonData.reward1;
        collected[1] = jsonData.reward2;
        collected[2] = jsonData.reward3;

        for (int i = 0; i < rewards.Length; i++) {
            if (jsonData.winCount >= rewards[i].winCount) {
                rewards[i].readyToCollectIndicator.SetActive(false);
                rewards[i].collectButton.interactable = true;

                if (collected[i] == true) {
                    rewards[i].completedLayer.SetActive(true);

                    rewards[i].collectButton.enabled = false;
                } else {
                    rewards[i].readyToCollectIndicator.SetActive(true);
                }
            } else {
                rewards[i].readyToCollectIndicator.SetActive(false);
                rewards[i].collectButton.interactable = false;
                rewards[i].completedLayer.SetActive(false);
            }
        }

        winCounterText.text = Mathf.Min(5, jsonData.winCount).ToString() + "/5";
        fillSlider.value = jsonData.winCount / 5f;


        int hours = 23 - PlayFabScript.timeAtLogin.Time.Hour;
        int mins = 60 - PlayFabScript.timeAtLogin.Time.Minute;

        timeLeftText.text = hours + "h " + mins + "min left";
    }

    /// <summary>
    /// Called by Gamesocket on win
    /// </summary>
    public void AddWin() {
        jsonData.winCount++;

        UpdatePlayerData();
    }

    /// <summary>
    /// Resetting the winReward json data on playfab weekly
    /// </summary>
    void ResetDailyData() {
        if (jsonData == null) {
            jsonData = new BattleRoadJson();
        }

        jsonData.winCount = 0;

        CultureInfo c = new CultureInfo("en-US");
        jsonData.resetDate = PlayFabScript.timeAtLogin.Time.ToString("d", c);
        jsonData.reward1 = jsonData.reward2 = jsonData.reward3 = false;

        UpdatePlayerData();

        SetVisuals();
    }

    /// <summary>
    /// Attached to the collect button -> sending Cloudscript Call to collect reward
    /// </summary>
    /// <param name="chestId"></param>
    public void ClaimReward() {
        if (requestInProgress) return;
        requestInProgress = true;

        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest() {
            FunctionName = "collectBattleRoad",
            GeneratePlayStreamEvent = true,
        };
        PlayFabClientAPI.ExecuteCloudScript(request, onClaimResult, OnPlayFabError);
    }

    private void OnPlayFabError(PlayFabError obj) {
        Debug.LogError(obj.GenerateErrorReport());
    }

    private void onClaimResult(ExecuteCloudScriptResult obj) {
        object successObj, reward;
        bool success;

        JsonObject jsonResult = (JsonObject)obj.FunctionResult;
        jsonResult.TryGetValue("success", out successObj);

        bool.TryParse(successObj.ToString(), out success);

        if (!success) return;

        jsonResult.TryGetValue("reward", out reward);

        /*if (!jsonData.reward1) {
            ExecuteCloudScriptResult res = new ExecuteCloudScriptResult();
            res.FunctionResult = reward;

            ChestOpening._instance.SetBronzeChest();
            ChestOpening._instance.OnClaimResult(res);


            jsonData.reward1 = true;
        } else*/
        if (!jsonData.reward1) {
            int amount;
            int.TryParse(reward.ToString(), out amount);
            CurrencyManager._instance.AddCrystals(amount);

            jsonData.reward1 = true;
            UI_VFX_FromTo._instance.CrystalIncrease();
        } else if (!jsonData.reward2) {
            ExecuteCloudScriptResult res = new ExecuteCloudScriptResult();
            res.FunctionResult = reward;

            ChestOpening._instance.SetGoldChest();
            ChestOpening._instance.OnClaimResult(res);

            jsonData.reward2 = true;
        }

        SetVisuals();
        requestInProgress = false;
    }

    /// <summary>
    /// Updating Playfab player data with the data of WeeklyWinClass
    /// </summary>
    void UpdatePlayerData() {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("battleRoad", JsonUtility.ToJson(jsonData));

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
            Data = data,
            Permission = UserDataPermission.Public
        }, resultUpdate => SetVisuals(),
       error => Debug.Log("Got error setting user data")
    );
    }
}

/// <summary>
/// Metaclass for JSON data of weekly win rewards.
/// </summary>
public class BattleRoadJson {
    public int winCount = 0;
    public string resetDate;
    public bool reward1 = false, reward2 = false, reward3 = false;
}