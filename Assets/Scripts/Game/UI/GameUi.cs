using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUi : MonoBehaviour {
    public static GameUi _instance;


    [Header("Overview")]

    [SerializeField] TMP_Text waveInfoText;

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        playerStatuses = new Dictionary<int, PlayerStatus>();

        waveInfoText.text = "Waiting for players...";
    }

    public void SetWaveInfo(int time, string msg) {
        waveInfoText.text = msg + "\n" + time;
    }

    #region Bottom UI Group
    [Space(15)]
    [Header("Bottom")]
    public TMP_Text goldText;
    public TMP_Text manaText;

    public TMP_Text goldIncomeText;
    public TMP_Text manaIncomeText;

    public Transform unitHolder;
    public PurchaseUnit pUnit;

    public Transform buildingHolder;
    public PurchaseBuilding pBuilding;

    /// <summary>
    /// Initializing the purchasable units and buildings in the bottom UI
    /// </summary>
    /// <param name="rd"></param>
    public void SetBottomUi(RaceData rd) {
        foreach (UnitData ud in rd.units.OrderBy(x => x.stats.levels[0].price)) {
            var unit = Instantiate(pUnit, unitHolder);
            unit.Init(ud);
        }

        foreach (BuildingData bd in rd.buildings.OrderBy(x => x.stats.levels[0].price)) {
            var building = Instantiate(pBuilding, buildingHolder);
            building.Init(bd);
        }
    }


    public void SetIncomeTexts() {
        int gold = GameManager._instance.localPlayer.goldIncomePerWave;
        int mana = GameManager._instance.localPlayer.manaIncomePerWave;

        goldIncomeText.text = (gold > 0 ? "+" : "") + gold + " /wave";
        manaIncomeText.text = (mana > 0 ? "+" : "") + mana + " /wave";
    }

    #endregion


    #region General
    /// <summary>
    /// Exiting to Main Menu
    /// </summary>
    public void Exit() {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Opening settings window
    /// </summary>
    public void OpenSettings() {
        Settings._instance.settingsWindow.SetActive(true);
    }

    #endregion

    #region Player Statuses
    [Space(12)]
    [Header("Player Statuses")]

    [SerializeField] PlayerStatus playerStatusPrefab;
    [SerializeField] Transform playerStatusHolder;

    Dictionary<int, PlayerStatus> playerStatuses;


    public void AddPlayerStatus(PlayerManager p) {
        var newP = Instantiate(playerStatusPrefab, playerStatusHolder);
        newP.InitUi(p.playerName, p.raceData, p.isLocal ? Color.white : Color.red, p.id);

        if (p.isLocal) {
            newP.transform.SetAsFirstSibling();
            newP.transform.localScale = Vector3.one * 1.25f;
            newP.GetComponent<RectTransform>().sizeDelta = new Vector2(125, 125);
        }

        newP.gameObject.SetActive(true);
        playerStatuses.Add(p.id, newP);

        newP.nameText.color = p.playerColor;
    }

    public void SetHp(int id, int hp) {
        playerStatuses[id].SetHp(hp);
    }

    #endregion

}
