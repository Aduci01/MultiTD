using UnityEngine;
using System.Collections;
using System.Net;
using System;
using UnityEngine.SceneManagement;

public class ClientHandler : MonoBehaviour {
    public static void Welcome(Packet packet) {
        string msg = packet.ReadString();
        short myId = packet.ReadShort();

        Client._instance.myId = myId;

        if (SceneManager.GetActiveScene().buildIndex == 1) {
            ClientSend.MasterWelcomeReceived();
        } else
            ClientSend.WelcomeReceived();

        Client._instance.udp.Connect(((IPEndPoint)Client._instance.tcp.socket.Client.LocalEndPoint).Port);

        Debug.Log($"[SERVER] : {msg}");

        if (LoadingScreen._instance != null)
            LoadingScreen._instance.doneProgress++;
    }

    public static void SpawnPlayer(Packet packet) {
        short id = packet.ReadShort();
        string username = packet.ReadString();
        string raceId = packet.ReadString();

        GameManager._instance.SpawnPlayer(id, username, raceId);
    }

    public static void PlayerDisconnected(Packet packet) {
        short id = packet.ReadShort();

        Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);
    }

    public static void RoomPort(Packet packet) {
        Matchmaking.gameRoomPort = packet.ReadInt();

        Debug.Log("Game started on " + Matchmaking.gameRoomPort);

        Client._instance.Disconnect();

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");

        Debug.Log(Matchmaking.gameRoomPort);
    }

    //Sent by server in every Second
    public static void Ping(Packet packet) {
        int gameTime = packet.ReadInt();
    }

    public static void GameOver(Packet packet) {
        //PlayerController._instance.enabled = false;
        GameManager.players[Client._instance.myId].gameObject.SetActive(false);

        Client._instance.Disconnect();

        //GameUi._instance.GameOver(packet.ReadInt());
    }

    public static void ReceiveChatMessage(Packet packet) {
        short fromId = packet.ReadShort();
        string msg = packet.ReadString();

        ChatManager._instance.ReceiveMessage(fromId, msg);
    }

    public static void GoldChanged(Packet packet) {
        int amount = packet.ReadInt();

        GameManager.players[Client._instance.myId].SetGoldCurrency(amount);
    }

    public static void ManaChanged(Packet packet) {
        int amount = packet.ReadInt();

        GameManager.players[Client._instance.myId].SetManaCurrency(amount);
    }

    public static void BuildingPlaced(Packet packet) {
        short id = packet.ReadShort();

        short serverId = packet.ReadShort();
        string buildingId = packet.ReadString();
        Vector3 pos = packet.ReadVector3();

        PlayerManager pm = GameManager.players[id];
        pm.BuildBuilding(serverId, buildingId, pos);
    }

    public static void UnitPlaced(Packet packet) {
        short id = packet.ReadShort();

        short serverId = packet.ReadShort();
        string buildingId = packet.ReadString();
        Vector3 pos = packet.ReadVector3();

        PlayerManager pm = GameManager.players[id];
        pm.BuildUnit(serverId, buildingId, pos);
    }

    public static void PrewaveTime(Packet packet) {
        short time = packet.ReadShort();

        GameManager._instance.PrewaveTime(time);
    }

    public static void SpawnEnemy(Packet packet) {
        short playerId = packet.ReadShort();

        string enemyId = packet.ReadString();
        short serverId = packet.ReadShort();

        GameManager.players[playerId].SpawnEnemy(DataCollection.enemyDb[enemyId], serverId);
    }

    public static void EntityHp(Packet packet) {
        short serverId = packet.ReadShort();
        int hp = packet.ReadShort();

        if (GameManager._instance.allEntities.ContainsKey(serverId))
            GameManager._instance.allEntities[serverId].SetHealth(hp);
    }

    public static void UpgradeEntity(Packet packet) {
        Entity e = GameManager._instance.allEntities[packet.ReadShort()];
        e.Upgrade();

        EntityViewer._instance.window.SetActive(false);
        CauldronUi._instance.window.SetActive(false);
    }

    public static void PlayerHp(Packet packet) {
        short playerId = packet.ReadShort();
        short hp = packet.ReadShort();

        GameManager.players[playerId].SetHp(hp);
    }

    public static void MercenaryPurchase(Packet packet) {
        short playerId = packet.ReadShort();
        string enemyId = packet.ReadString();

        GameManager.players[playerId].AddMercenary(enemyId);
    }

    public static void RemoveEntity(Packet packet) {
        short playerId = packet.ReadShort();
        short serverId = packet.ReadShort();

        GameManager.players[playerId].RemoveEntity(serverId);
    }

    public static void SyncTarget(Packet packet) {
        short serverId = packet.ReadShort();
        bool nullTarget = packet.ReadBool();
        short targetId = packet.ReadShort();

        if (GameManager._instance.allEntities.ContainsKey(serverId))
            GameManager._instance.allEntities[serverId].SetTarget(targetId, nullTarget);
    }

    public static void SummonEnemy(Packet packet) {
        short playerId = packet.ReadShort();

        string enemyId = packet.ReadString();
        short serverId = packet.ReadShort();

        short posX = packet.ReadShort(), posZ = packet.ReadShort();

        GameManager.players[playerId].SpawnEnemy(DataCollection.enemyDb[enemyId], serverId, posX, posZ);

    }
}
