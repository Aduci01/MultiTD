using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour {
    public static Client _instance;
    public static int dataBufferSize = 4096;

    public static string ip = "127.0.0.1"; //"18.198.145.4"
    public static int port = 26955;

    public short myId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;

    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake() {
        if (_instance != this) {
            _instance = this;

            return;
        }
    }

    private void OnApplicationQuit() {
        Disconnect();
    }

    private void Start() {

    }

    float timer;
    private void Update() {
        timer += Time.deltaTime;
    }

    public void ConnectToServer() {
        tcp = new TCP();
        udp = new UDP();

        InitializeClientData();

        isConnected = true;

        tcp.Connect();
    }

    public class TCP {
        public TcpClient socket;

        private NetworkStream stream;

        private Packet receivedData;

        private byte[] receiveBuffer;

        public void Connect() {
            socket = new TcpClient {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(ip, port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult result) {
            socket.EndConnect(result);

            if (!socket.Connected) {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet packet) {
            try {
                if (socket != null) {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            } catch (Exception ex) {
                Debug.LogError($"Error sending data to server: {ex}");
            }
        }


        private void ReceiveCallback(IAsyncResult result) {
            try {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0) {
                    _instance.Disconnect();

                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receivedData.Reset(HandleData(data));

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            } catch {
                Disconnect();
            }
        }

        private bool HandleData(byte[] data) {
            int packetLength = 0;

            receivedData.SetBytes(data);

            if (receivedData.UnreadLength() >= 4) {
                packetLength = receivedData.ReadInt();

                if (packetLength <= 0) {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength()) {
                byte[] packetBytes = receivedData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() => {
                    using (Packet packet = new Packet(packetBytes)) {
                        int packetId = packet.ReadInt();

                        packetHandlers[packetId](packet);
                    }
                });


                packetLength = 0;
                if (receivedData.UnreadLength() >= 4) {
                    packetLength = receivedData.ReadInt();

                    if (packetLength <= 0) {
                        return true;
                    }
                }
            }


            if (packetLength <= 1) {
                return true;
            }

            return false;
        }

        private void Disconnect() {
            _instance.Disconnect();

            stream = null;
            receiveBuffer = null;
            receivedData = null;
            socket = null;
        }
    }

    public class UDP {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP() {
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public void Connect(int localPort) {
            socket = new UdpClient(localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet packet = new Packet()) {
                SendData(packet);
            }
        }

        public void SendData(Packet _packet) {
            try {
                _packet.InsertInt(_instance.myId);

                if (socket != null) {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }

            } catch (Exception _ex) {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result) {
            try {
                byte[] data = socket.EndReceive(result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4) {
                    _instance.Disconnect();

                    return;
                }

                HandleData(data);
            } catch {
                Disconnect();
            }
        }

        private void HandleData(byte[] _data) {
            using (Packet _packet = new Packet(_data)) {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() => {
                using (Packet _packet = new Packet(_data)) {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }

        private void Disconnect() {
            _instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }

    private void InitializeClientData() {
        packetHandlers = new Dictionary<int, PacketHandler>() {
            { (int)ServerPackets.welcome, ClientHandler.Welcome },
            { (int)ServerPackets.playerDisconnected, ClientHandler.PlayerDisconnected },
            { (int)ServerPackets.ping, ClientHandler.Ping },

            { (int)ServerPackets.gameOver, ClientHandler.GameOver },
            { (int)ServerPackets.spawnPlayer, ClientHandler.SpawnPlayer },
            { (int)ServerPackets.chatReceived, ClientHandler.ReceiveChatMessage },
            { (int)ServerPackets.preWaveTime, ClientHandler.PrewaveTime },

            { (int)ServerPackets.goldChanged, ClientHandler.GoldChanged },
            { (int)ServerPackets.manaChanged, ClientHandler.ManaChanged },

            { (int)ServerPackets.unitPlaced, ClientHandler.UnitPlaced },
            { (int)ServerPackets.buildingPlaced, ClientHandler.BuildingPlaced },
            { (int)ServerPackets.removeEntity, ClientHandler.RemoveEntity },

            { (int)ServerPackets.spawnEnemy, ClientHandler.SpawnEnemy },
            { (int)ServerPackets.entityHp, ClientHandler.EntityHp },
            { (int)ServerPackets.summonEnemy, ClientHandler.SummonEnemy },

            { (int)ServerPackets.upgradeEntity, ClientHandler.UpgradeEntity },

            { (int)ServerPackets.playerHp, ClientHandler.PlayerHp },

            { (int)ServerPackets.mercenaryPurchase, ClientHandler.MercenaryPurchase },

            { (int)ServerPackets.syncTarget, ClientHandler.SyncTarget },

            { (int)ServerPackets.roomPort, ClientHandler.RoomPort },

        };

        Debug.Log("Initializing Packets..");
    }

    public void Disconnect() {
        if (!isConnected) return;

        isConnected = false;
        tcp.socket.Close();

        if (udp.socket != null)
            udp.socket.Close();

        Debug.Log("Disconnected from server");
    }
}
