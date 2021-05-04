using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;

public class UIManager : MonoBehaviour {
    public static UIManager _instance;

    [Header("MENU")]
    public GameObject startMenu, classSelection;

    public TMP_Text username;
    public TMP_Text trophyText;

    public TMP_Text levelText, xpText;
    public Slider levelFillBar;

    public Image selectedRaceImage;
    public TMP_Text selectedRaceName;

    public GameObject notmatchingVersionWindow;

    [Space(10)]
    [Header("TitleNews")]
    public TMP_Text newsTitle;
    public TMP_Text newsBody;

    private void Awake() {
        if (!PlayFab.PlayFabClientAPI.IsClientLoggedIn())
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);

        if (_instance == null) {
            _instance = this;

            return;
        }

        Destroy(gameObject);
    }

    private void Start() {
        PlayerData.selectedRaceId = PlayerPrefs.GetString("SELECTED_RACE", "Humans");

        if (!PlayFabScript._instance.firstPlayfabCallRequested) {
            PlayFabScript._instance.GetServerTime();
        }

        PlayFabScript._instance.GetTitleNews(newsTitle, newsBody);
    }

    #region Menu
    [Space(10)]

    [Header("Popup Windows")]
    [SerializeField] GameObject inQueuePvP;
    [SerializeField] TMP_Text estimatedTime, deltaTimeInQueueText, playersInQueueText;

    public void SetPlayersInQueueText(int n) {
        playersInQueueText.text = (n + 1) + " Players are waiting";
    }

    public void ChangePlayerName(TMP_InputField input) {
        if (input.text.Length < 2) {
            MessageHandler._instance.ShowMessage("Name must be at least 2 characters long", 1f, Color.red);
        }

        PlayFabScript._instance.UpdateDisplayName(input.text);

        username.text = input.text;
        PlayerPrefs.SetString("PLAYER_NAME", input.text);
    }

    public void StartRanked() {
        inQueuePvP.SetActive(false);
        LoadingScreen._instance.StartLoading(10, 1);

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");
        //ClientSend.JoinRoom();
    }

    public void QueueStart(string queueName) {
        inQueuePvP.SetActive(true);

        estimatedTime.text = "Estimated time: " + Random.Range(23, 35) + "sec";

        Matchmaking._instance.CreateTicket(queueName);

        PlayerData.playerName = username.text;
    }

    public void QueueStop() {
        if (inQueuePvP != null)
            inQueuePvP.SetActive(false);

        Client._instance.Disconnect();
    }

    public void SetQueueTimer(int sec) {
        int mins = sec / 60;
        int secs = sec - mins * 60;

        deltaTimeInQueueText.text = string.Format("{0:00}:{1:00}", mins, secs);
    }


    public void SetData(Dictionary<string, UserDataRecord> userReadOnlyData) {
        if (userReadOnlyData.ContainsKey("level")) {
            PlayerData.PlayerLVL = int.Parse(userReadOnlyData["level"].Value);
        } else {
            PlayerData.PlayerLVL = 1;
            Debug.LogWarning("userReadOnlyData from playfab do not have 'level' key");
        }

        if (userReadOnlyData.ContainsKey("xp")) {
            PlayerData.PlayerXP = int.Parse(userReadOnlyData["xp"].Value);
        } else {
            PlayerData.PlayerXP = 0;
            Debug.LogWarning("userReadOnlyData from playfab do not have 'xp' key");
        }

        SetUI();
    }

    public void SetUI() {
        levelText.text = PlayerData.PlayerLVL + "";
        levelFillBar.value = PlayerData.PlayerXP / (float)PlayerData.GetXpForNextLevel(PlayerData.PlayerLVL);

        xpText.text = PlayerData.PlayerXP + "/" + PlayerData.GetXpForNextLevel(PlayerData.PlayerLVL);
    }

    public void SetRace(RaceData rd) {
        selectedRaceImage.sprite = rd.icon;
        selectedRaceName.text = rd.displayName;
    }

    #endregion

    public void OpenSettings() {
        Settings._instance.settingsWindow.SetActive(true);
    }

    public void GoToGameScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void Exit() {
        Application.Quit();
    }

}
