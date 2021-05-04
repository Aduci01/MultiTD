using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_VFX_FromTo : MonoBehaviour {
    public static UI_VFX_FromTo _instance;

    public IncreaseUiVfx xpIncrease, coinIncrease, crystalIncrease;

    public iTween.EaseType easeType;

    public float time;
    public float rate;
    public float amount;

    private void Awake() {
        _instance = this;
    }

    public void XpIncrease() {
        StartCoroutine(FromToCo(xpIncrease));
    }

    public void CoinIncrease() {
        StartCoroutine(FromToCo(coinIncrease));
    }

    public void CrystalIncrease() {
        StartCoroutine(FromToCo(crystalIncrease));
    }

    IEnumerator FromToCo(IncreaseUiVfx iuv) {
        for (int i = 0; i < amount; i++) {
            iuv.NumberIncreaseCaller(0.5f);

            var vfx = Instantiate(iuv.UiVfx, iuv.origin.transform.position, Quaternion.identity) as GameObject;
            vfx.transform.SetParent(iuv.origin.transform);
            vfx.transform.localScale = Vector3.one;

            iTween.MoveTo(vfx, iTween.Hash("position", iuv.destination.transform.position + iuv.offset, "easetype", easeType, "ignoretimescale", true, "time", time));

            Destroy(vfx, time + 1);

            yield return new WaitForSeconds(rate);
        }
    }
}
