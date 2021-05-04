﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageHandler : MonoBehaviour {

    public static MessageHandler _instance;

    public Text messageText;
    public Image backgroundImage;

    string prevText = "";

    private void Awake() {
        if (_instance == null)
            _instance = this;
    }

    public void ShowMessage(string text, float duration, Color c) {
        if (prevText == text) return;

        prevText = text;
        messageText.text = text;
        StartCoroutine(MessageCoroutine(text, duration, c));
    }

    IEnumerator MessageCoroutine(string text, float duration, Color c) {
        Image[] images = backgroundImage.GetComponentsInChildren<Image>();
        float t = 0;

        while (t < 1) {
            t += Time.deltaTime * 3f;
            messageText.color = new Color(c.r, c.g, c.b, t);
            backgroundImage.color = new Color(0, 0, 0, t / 1.20f);

            foreach (Image i in images) {
                i.color = new Color(i.color.r, i.color.g, i.color.b, t / 1.20f);
            }

            yield return null;
        }

        yield return new WaitForSeconds(duration);

        while (t > 0) {
            t -= Time.deltaTime * 3f;
            messageText.color = new Color(c.r, c.g, c.b, t);
            backgroundImage.color = new Color(0, 0, 0, t / 1.20f);

            foreach (Image i in images) {
                i.color = new Color(i.color.r, i.color.g, i.color.b, t / 1.20f);
            }

            yield return null;
        }

        messageText.color = new Color(1, 1, 1, 0);
        backgroundImage.color = new Color(0, 0, 0, 0);

        foreach (Image i in images) {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        }


        prevText = "";
    }
}
