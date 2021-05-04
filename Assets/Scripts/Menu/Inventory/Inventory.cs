using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collection of the Races and collectible items and skins in the Menu
/// </summary>
public class Inventory : MonoBehaviour {
    public static Inventory _instance;

    private void Awake() {
        _instance = this;
    }

    // Start is called before the first frame update
    IEnumerator Start() {
        raceUis = new List<RaceUi>();

        //Waiting for data to be fetched from Playfab
        while (DataCollection.raceDb.Count == 0 || DataCollection.buildingDb.Count == 0 || DataCollection.unitDb.Count == 0) {
            yield return new WaitForSeconds(0.5f);
        }

        SetUpRaceUI();
    }


    #region Race
    public Transform raceTransform;
    public List<RaceUi> raceUis;
    public RaceUi raceUiPrefab;

    void SetUpRaceUI() {
        foreach (RaceData rd in DataCollection.raceDb.Values) {
            var r = Instantiate(raceUiPrefab, raceTransform);
            r.SetRace(rd);

            raceUis.Add(r);
            r.gameObject.SetActive(true);

            if (PlayerPrefs.GetString("CURRENT_RACE", "Humans") == rd.id)
                PlayerData.SetRace(rd);
        }
    }

    #endregion
}
