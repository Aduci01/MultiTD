using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabScript : MonoBehaviour {
    public static PlayFabScript _instance;
    public static string VERSION = "1.0";

    public static Dictionary<string, UserDataRecord> userData;
    public static GetTimeResult timeAtLogin;

    public bool firstPlayfabCallRequested;

    void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        //Exit to Preloader scene if player is not logged in
        if (!PlayFabClientAPI.IsClientLoggedIn()) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        //GetServerTime();
    }

    #region public Getter methods for Playfab data
    public void GetCombinedData() {
        GetPlayerCombinedInfoRequestParams param = new GetPlayerCombinedInfoRequestParams {
            GetUserAccountInfo = true,
            GetPlayerStatistics = true,
            GetUserInventory = false,
            GetUserVirtualCurrency = true,
            GetUserData = true,
            GetUserReadOnlyData = true,
            GetTitleData = true
        };

        GetPlayerCombinedInfoRequest request = new GetPlayerCombinedInfoRequest {
            InfoRequestParameters = param
        };

        PlayFabClientAPI.GetPlayerCombinedInfo(request, GetCombinedResult, error => { Debug.LogError("GetCombined error" + error.Error); });
    }

    public void GetCurrency() {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result => GetCurrenciesResult(result.VirtualCurrency, result.VirtualCurrencyRechargeTimes), error => { Debug.LogError(error.Error); });
    }

    public void GetPlayerReadOnlyData() {

    }

    public void GetServerTime() {
        if (firstPlayfabCallRequested) return;

        firstPlayfabCallRequested = true;

        DataCollection.GetData();

        PlayFabClientAPI.GetTime(
            new GetTimeRequest(),
            result => {
                timeAtLogin = result;

                GetCombinedData();
            },
            error => {
                Debug.LogWarning("Something went wrong:");
                Debug.LogError(error.GenerateErrorReport());
            }
        );
    }
    #endregion

    #region Results of Playfab calls
    private void GetCombinedResult(GetPlayerCombinedInfoResult result) {
        GetAccountInfoResult(result.InfoResultPayload.AccountInfo);
        GetStatisticsResult(result.InfoResultPayload.PlayerStatistics);
        GetCurrenciesResult(result.InfoResultPayload.UserVirtualCurrency, result.InfoResultPayload.UserVirtualCurrencyRechargeTimes);
        GetUserDataResult(result.InfoResultPayload.UserData);
        GetUserReadOnlyData(result.InfoResultPayload.UserReadOnlyData);

        GetTitleData(result.InfoResultPayload.TitleData);

        LoadingScreen._instance.doneProgress++;
    }

    private void GetTitleData(Dictionary<string, string> titleData) {
        if (titleData.ContainsKey("WaveInfo")) {
            DataCollection.waves = JsonUtility.FromJson<WaveParent>(titleData["WaveInfo"]);
        }

        if (titleData.ContainsKey("VERSION")) {
            if (!titleData["VERSION"].Equals(VERSION)) UIManager._instance.notmatchingVersionWindow.SetActive(true);
        }
    }

    private void GetStatisticsResult(List<StatisticValue> stats) {
        foreach (var stat in stats) {
            if (stat.StatisticName == "Ranked") {
                PlayerData.playerTrophies = stat.Value;
                UIManager._instance.trophyText.text = stat.Value + "";
            }
        }
    }

    private void GetUserDataResult(Dictionary<string, UserDataRecord> uData) {
        userData = uData;

        BattleRoad._instance.SetData();
    }

    private void GetUserReadOnlyData(Dictionary<string, UserDataRecord> userReadOnlyData) {
        UIManager._instance.SetData(userReadOnlyData);

        if (userReadOnlyData.ContainsKey("matchData"))
            EndGame._instance.SetUpWindow(userReadOnlyData["matchData"]);
    }

    private void GetAccountInfoResult(UserAccountInfo result) {
        PlayerData.playFabId = result.PlayFabId;
        PlayerData.playerName = GetNameResult(result);

        UIManager._instance.username.text = PlayerData.playerName;
    }

    public void GetCurrenciesResult(Dictionary<string, int> result, Dictionary<string, VirtualCurrencyRechargeTime> times) {
        CurrencyManager._instance.SetCoins(result["CO"]);
        CurrencyManager._instance.SetCrystals(result["CR"]);

        ////FreeChest._instance.SetValues(result["FC"], times["FC"]);
    }

    private string GetNameResult(UserAccountInfo result) {
        string playerName;
        /*if (result.TitleInfo.DisplayName != null) {
            playerName = result.TitleInfo.DisplayName;
        } else {
            playerName = "Player";
            //MenuManager._instance.nameChangeWindow.SetActive(true);
        }*/

        playerName = result.Username;
        return char.ToUpper(playerName[0]) + playerName.Substring(1);
        //return playerName;
    }
    #endregion

    public void UpdateDisplayName(string newName) {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {
            DisplayName = newName
        }, result => {
            Debug.Log("The player's display name is now: " + result.DisplayName);

            PlayerData.playerName = result.DisplayName;
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    public static void PlayfabEvent(string eventName, Dictionary<string, object> data = null) {
        PlayFabClientAPI.WritePlayerEvent(
            new WriteClientPlayerEventRequest() {
                Body = data,
                EventName = eventName
            },
            (result) => {
                Debug.Log("[Playfab] Playfab event " + eventName + "Success");
            },
            (error) => {
                Debug.LogError(error.GenerateErrorReport());
            }
        );
    }

    public void GetTitleNews(TMPro.TMP_Text title, TMPro.TMP_Text body) {
        PlayFabClientAPI.GetTitleNews(new GetTitleNewsRequest(), result => {
            TitleNewsItem news = result.News.OrderByDescending(x => x.Timestamp).FirstOrDefault();
            title.text = news.Title;
            body.text = news.Body;
        }, error => Debug.LogError(error.GenerateErrorReport()));

    }
}
