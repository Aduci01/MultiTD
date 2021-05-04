using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveInfo : MonoBehaviour {
    public static WaveInfo _instance;

    public WaveData[] waves;

    public Text waveText;
    public Text[] enemyTexts;

    public GameObject window;

    private void Awake() {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        waves = DataCollection.waves.waves;

        SetWaveData();
    }

    /// <summary>
    /// Setting wave info
    /// </summary>
    public void SetWaveData() {
        int waveNumber = Mathf.Max(GameManager._instance.waveNumber, 0);

        WaveData w = waves[waveNumber];
        waveText.text = "Wave " + (waveNumber + 1);

        for (int i = 0; i < enemyTexts.Length; i++) {
            if (w.wavePart.Length <= i) {
                enemyTexts[i].gameObject.SetActive(false);
            } else {
                enemyTexts[i].gameObject.SetActive(true);
                enemyTexts[i].text = "x" + w.wavePart[i].amount + " " + DataCollection.enemyDb[w.wavePart[i].enemyType].displayName;
            }
        }
    }

    public void OnClickToggle() {
        window.SetActive(!window.activeSelf);
    }
}
