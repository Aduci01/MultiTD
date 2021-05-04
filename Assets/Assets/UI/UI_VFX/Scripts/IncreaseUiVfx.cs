using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IncreaseUiVfx : MonoBehaviour {
    public GameObject UiVfx;
    public GameObject origin;
    public GameObject destination;

    public Vector3 offset;


    public void NumberIncreaseCaller(float delay) {
        StartCoroutine(NumberIncrease(delay));
    }

    IEnumerator NumberIncrease(float delay) {
        yield return new WaitForSeconds(delay);
        //iTween.PunchScale(icon, new Vector2(.15f, .15f), 0.2f);
    }
}
