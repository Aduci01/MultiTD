using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : MonoBehaviour {
    public static OverlayManager _instance;

    public GameObject circleExplosion;
    public GameObject buttonExplosion;

    public GameObject levelupWindow;

    private void Awake() {
        if (_instance != null) {
            return;
        }

        _instance = this;
    }

    public void ButtonExplosion(Vector3 position) {
        buttonExplosion.transform.position = position;
        buttonExplosion.SetActive(true);
    }


    public void PlayerLevelUp() {
        levelupWindow.SetActive(true);

        CurrencyManager._instance.AddCrystals(10);
    }
}
