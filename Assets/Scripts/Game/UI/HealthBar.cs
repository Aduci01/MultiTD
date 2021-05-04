using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    public Slider hpBar;

    public void SetValue(float v) {
        hpBar.value = v;
    }

    // Update is called once per frame
    void Update() {
        transform.eulerAngles = new Vector3(60, 0, 0);
    }
}
