using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerObserver : MonoBehaviour {
    public static ServerObserver _instance;

    public Text msText;
    float msCounter, msTimer;

    private void Awake() {
        _instance = this;
    }

    private void Update() {
        msCounter += Time.deltaTime;
        msTimer += Time.deltaTime;
    }

    public void UpdateMs() {
        if (msTimer > 2f) {
            msText.text = "ms: " + (int)(msCounter * 1000);
            msTimer = 0;
        }

        msCounter = 0;
    }
}
