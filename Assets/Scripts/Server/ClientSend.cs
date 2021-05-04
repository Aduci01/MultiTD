using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour {
    private static void SendTCPData(Packet packet) {
        packet.WriteLength();
        Client._instance.tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet) {
        packet.WriteLength();
        Client._instance.udp.SendData(packet);
    }

    #region Packets
    public static void MasterWelcomeReceived() {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived)) {
            packet.Write(Client._instance.myId);
            packet.Write(PlayerData.playerName);

            SendTCPData(packet);
        }
    }

    public static void WelcomeReceived() {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived)) {
            packet.Write(Client._instance.myId);
            packet.Write(PlayerData.playerName);

            if (PlayerData.raceData != null) {
                packet.Write(PlayerData.raceData.id);
            } else {
                packet.Write("Humans");
            }

            packet.Write(PlayerData.playFabId);

            SendTCPData(packet);
        }
    }

    public static void JoinRoom() {
        using (Packet packet = new Packet((int)ClientPackets.joinRoom)) {
            packet.Write(Matchmaking._instance.matchID);

            SendTCPData(packet);
        }
    }

    public static void SendChatMessage(string msg) {
        using (Packet packet = new Packet((int)ClientPackets.chatMessage)) {
            packet.Write(msg);

            SendTCPData(packet);
        }
    }

    public static void PlacementRequestUnit(UnitData data, Vector3 pos) {
        using (Packet packet = new Packet((int)ClientPackets.unitPlacementRequest)) {
            packet.Write(data.id);
            packet.Write(pos);

            SendTCPData(packet);
        }
    }

    public static void PlacementRequestBuilding(BuildingData data, Vector3 pos) {
        using (Packet packet = new Packet((int)ClientPackets.buildingPlacementRequest)) {
            packet.Write(data.id);
            packet.Write(pos);

            SendTCPData(packet);
        }
    }

    public static void UpgradeRequest(short serverId) {
        using (Packet packet = new Packet((int)ClientPackets.upgradeRequest)) {
            packet.Write(serverId);

            SendTCPData(packet);
        }
    }

    public static void MercenaryRequest(EnemyData data) {
        using (Packet packet = new Packet((int)ClientPackets.mercenaryRequest)) {
            packet.Write(data.id);

            SendTCPData(packet);
        }
    }

    public static void CauldronEvent(Building b, int amount) {
        using (Packet packet = new Packet((int)ClientPackets.cauldronEvent)) {
            packet.Write(b.serverId);
            packet.Write(amount);

            SendTCPData(packet);
        }
    }

    public static void SellEntityRequest(Entity entity) {
        using (Packet packet = new Packet((int)ClientPackets.sellEntityRequest)) {
            packet.Write(entity.serverId);

            SendTCPData(packet);
        }
    }

    #endregion
}
