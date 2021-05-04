using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.MultiplayerModels;
using PlayFab.ClientModels;
using EntityKey = PlayFab.MultiplayerModels.EntityKey;
using System;

public class Matchmaking : MonoBehaviour {
    public static Matchmaking _instance;

    public string queueName = "pvp";
    public float queueTime = 600;

    public string status;
    public string matchID;

    private string ticketID = "";
    private float requestTimer = 0;
    private float queueTimer = 0;
    private bool inQueue = false;

    public static int gameRoomPort = 26956;

    void Awake() {
        _instance = this;
    }

    public void Update() {
        if (!inQueue) return;

        requestTimer += Time.deltaTime;
        if (ticketID != "" && requestTimer >= 6) {
            requestTimer = 0;

            PlayFabMultiplayerAPI.GetMatchmakingTicket(
                new GetMatchmakingTicketRequest {
                    TicketId = ticketID,
                    QueueName = queueName,
                },
                (GetMatchmakingTicketResult result) => {
                    status = result.Status;
                    matchID = result.MatchId;

                    if (status == "Matched") {
                        PlayFabMultiplayerAPI.GetMatch(new GetMatchRequest {
                            MatchId = matchID,
                            QueueName = queueName,
                        },
                        this.OnGetMatch,
                        this.OnMatchmakingError);


                    } else {
                        GetQueueStatisticsRequest statRequest = new GetQueueStatisticsRequest {
                            QueueName = queueName
                        };
                        PlayFabMultiplayerAPI.GetQueueStatistics(statRequest, (GetQueueStatisticsResult r) => { UIManager._instance.SetPlayersInQueueText((int)r.NumberOfPlayersMatching); }, (PlayFabError obj) => { });
                    }

                    if (status == "Canceled") {
                        inQueue = false;
                        UIManager._instance.QueueStop();
                    }
                },
                this.OnMatchmakingError);
        }

        if (inQueue) {
            queueTimer += Time.deltaTime;
            UIManager._instance.SetQueueTimer((int)queueTimer);
        }
    }

    private void OnGetMatch(GetMatchResult result) {
        inQueue = false;
        UIManager._instance.StartRanked();

        Client.ip = result.ServerDetails.IPV4Address;

        Client.port = result.ServerDetails.Ports[0].Num;
        gameRoomPort = result.ServerDetails.Ports[0].Num;
    }

    public void CreateTicket(string queue) {
        GetQueueStatisticsRequest statRequest = new GetQueueStatisticsRequest {
            QueueName = queue
        };
        PlayFabMultiplayerAPI.GetQueueStatistics(statRequest, (GetQueueStatisticsResult result) => { UIManager._instance.SetPlayersInQueueText((int)result.NumberOfPlayersMatching); }, (PlayFabError obj) => { });

        queueTimer = 0;
        queueName = queue;
        inQueue = true;

        PlayFabMultiplayerAPI.CreateMatchmakingTicket(
            new CreateMatchmakingTicketRequest {
                // The ticket creator specifies their own player attributes.
                Creator = new MatchmakingPlayer {
                    Entity = new EntityKey {
                        Id = PlayerData.playfabEntityID,
                        Type = PlayerData.playfabEntityType,
                    },

                    // Here we specify the creator's attributes.
                    Attributes = new MatchmakingPlayerAttributes {
                        DataObject = new {
                            Skill = PlayerData.playerTrophies,
                            Latencies = LatencyHandler.GetLatencies()
                        },
                    },
                },

                // Cancel matchmaking if a match is not found after some time seconds.
                GiveUpAfterSeconds = (int)(queueTime) + 10,

                // The name of the queue to submit the ticket into.
                QueueName = queueName,
            },

            // Callbacks for handling success and error.
            (CreateMatchmakingTicketResult result) => {
                ticketID = result.TicketId;
            },

            this.OnMatchmakingError);
    }


    public void OnMatchmakingError(PlayFabError error) {
        //TODO: Error message to user in case of too much join in
        inQueue = false;
        UIManager._instance.QueueStop();
        Debug.LogWarning("Matchmaking - Playfab Error");
        Debug.LogWarning(error);
    }

    private void OnDestroy() {
        if (ticketID != "") {
            PlayFabMultiplayerAPI.CancelAllMatchmakingTicketsForPlayer(
                new CancelAllMatchmakingTicketsForPlayerRequest {
                    QueueName = queueName
                },

                (CancelAllMatchmakingTicketsForPlayerResult result) => {
                    Debug.LogWarning("Matchmaking - cancel all success");
                },
                OnMatchmakingError);
        }
    }

    public void CancelMatchmaking() {
        PlayFabMultiplayerAPI.CancelMatchmakingTicket(
            new CancelMatchmakingTicketRequest {
                TicketId = ticketID,
                QueueName = queueName
            },

            (CancelMatchmakingTicketResult result) => {
                Debug.LogWarning("Matchmaking - cancel success");
                inQueue = false;
                UIManager._instance.QueueStop();
            },

            OnMatchmakingError);
    }
}
