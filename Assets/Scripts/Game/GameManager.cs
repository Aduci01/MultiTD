using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager _instance;

    public enum GameState { InWave, PreWave }
    public GameState state;
    public int waveNumber;

    public static Dictionary<short, PlayerManager> players;
    public Transform[] playerStartTransforms;

    public Color[] colors;
    public PlayerManager playerPrefab;

    public PlayerManager localPlayer;

    public Dictionary<short, Entity> allEntities;

    private void Awake() {
        _instance = this;
    }

    private IEnumerator Start() {
        players = new Dictionary<short, PlayerManager>();
        allEntities = new Dictionary<short, Entity>();

        yield return new WaitForSeconds(3.5f);

        Client.port = Matchmaking.gameRoomPort;
        Client._instance.ConnectToServer();

        StartCoroutine(BadConnectionTimeout());
    }

    public void SpawnPlayer(short id, string name, string raceId) {
        PlayerManager player;
        player = Instantiate(playerPrefab, playerStartTransforms[id].position, Quaternion.identity, playerStartTransforms[id]);

        if (id == Client._instance.myId) {
            player.isLocal = true;
            localPlayer = player;

            RtsCamera._instance.SetPositionToPlayer(id);
        } else {
            player.isLocal = false;
        }

        player.Initialize(id, name, raceId);

        players.Add(id, player);
    }

    public void PrewaveTime(short time) {
        GameUi._instance.SetWaveInfo(time, "Next wave in:");

        if (time <= -1) {
            state = GameState.InWave;
            GameUi._instance.SetWaveInfo(waveNumber + 1, "Wave");



            return;
        }

        if (state == GameState.InWave) {
            waveNumber++;

            state = GameState.PreWave;
            ResetAllEntities();

            WaveInfo._instance.SetWaveData();
        }
    }

    void ResetAllEntities() {
        foreach (PlayerManager p in players.Values) {
            if (p == null) continue;

            p.NewRound();
        }

        MercenaryManager._instance.SetLimitUi(0);
    }

    IEnumerator BadConnectionTimeout() {
        yield return new WaitForSeconds(9f);

        if (localPlayer == null) {
            if (MessageHandler._instance != null)
                MessageHandler._instance.ShowMessage("Cannot connect to Server", 3f, Color.red);

            SceneManager.LoadScene(0);
        }
    }
}
