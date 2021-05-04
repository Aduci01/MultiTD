using UnityEngine;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public class PlayFabLeaderboard : MonoBehaviour {
    public static PlayFabLeaderboard _instance;

    public static string pvp1v1Id = "1v1 PvP";

    public List<PlayerLeaderboardEntry> globalTopPlayers;

    [SerializeField] GameObject window;

    [SerializeField] LeaderboardPrefab playerPrefab, myPlayer;
    [SerializeField] Transform globalHolder;

    private void Awake() {
        _instance = this;
    }

    // Use this for initialization
    void Start() {
        if (!PlayFabClientAPI.IsClientLoggedIn()) return;

        GetLeaderboardData();
    }

    // Update is called once per frame
    void Update() {

    }

    public void GetLeaderboardData() {
        GetLeaderboardRequest request = new GetLeaderboardRequest {
            MaxResultsCount = 50,
            StartPosition = 0,
            StatisticName = pvp1v1Id
        };

        PlayFabClientAPI.GetLeaderboard(request, LeaderboardResult, error => Debug.Log(error.ErrorDetails));

        GetLeaderboardAroundPlayerRequest aroundRequest = new GetLeaderboardAroundPlayerRequest();
        aroundRequest.StatisticName = pvp1v1Id;
        aroundRequest.MaxResultsCount = 1;
        aroundRequest.PlayFabId = PlayerData.playFabId;

        PlayFabClientAPI.GetLeaderboardAroundPlayer(aroundRequest, AroundLeaderboardResult, error => Debug.Log(error.ErrorDetails));
    }

    private void LeaderboardResult(GetLeaderboardResult result) {
        globalTopPlayers = result.Leaderboard;

        foreach (PlayerLeaderboardEntry p in globalTopPlayers) {
            var g = Instantiate(playerPrefab);
            g.transform.SetParent(globalHolder);
            g.transform.localScale = Vector3.one;

            g.InitData(p.DisplayName, p.Position, p.StatValue);
        }
    }


    private void AroundLeaderboardResult(GetLeaderboardAroundPlayerResult result) {
        PlayerLeaderboardEntry p = result.Leaderboard[0];

        myPlayer.InitData(p.DisplayName, p.Position, p.StatValue);
    }

    public void OpenLeaderboard() {
        window.SetActive(true);
    }
}
