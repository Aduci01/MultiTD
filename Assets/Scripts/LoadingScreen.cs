using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour {
    public static LoadingScreen _instance;

    public int allProgress, doneProgress;

    [SerializeField] GameObject container, bgImage;
    [SerializeField] Image loadingBar;
    [SerializeField] Sprite[] backgrounds;

    [SerializeField] string[] tips;
    [SerializeField] TMP_Text tipText;

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLoading(float t, int allProgr) {
        doneProgress = 0;
        allProgress = allProgr;

        bgImage.GetComponent<Image>().sprite = backgrounds[Random.Range(0, backgrounds.Length)];
        container.SetActive(true);
        tipText.text = tips[Random.Range(0, tips.Length)];

        loadingBar.fillAmount = 0;
        StartCoroutine(FillBar(t));
    }

    /// <summary>
    /// Fill the bar with maximum waiting time.
    /// </summary>
    /// <param name="minSecs">Max</param>
    /// <returns></returns>
    IEnumerator FillBar(float minSecs) {
        yield return new WaitForEndOfFrame();

        float timer = 0;
        while (doneProgress < allProgress || timer < minSecs) {
            if (loadingBar.fillAmount < 1) {
                loadingBar.fillAmount += Time.deltaTime / 4f;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut() {
        var images = container.GetComponentsInChildren<Image>();
        var texts = container.GetComponentsInChildren<Text>();
        var t_texts = container.GetComponentsInChildren<TMP_Text>();

        Color[] starterColorImages = new Color[images.Length];
        Color[] starterColorTexts = new Color[texts.Length];

        for (int i = 0; i < images.Length; i++) {
            starterColorImages[i] = images[i].color;
        }

        for (int i = 0; i < texts.Length; i++) {
            starterColorTexts[i] = texts[i].color;
        }


        float t = 0, maxTime = 1;
        while (t < maxTime) {
            t += Time.deltaTime;

            foreach (Image image in images) {
                image.color = Color.Lerp(new Color(image.color.r, image.color.g, image.color.b, 1), new Color(image.color.r, image.color.g, image.color.b, 0), t / maxTime);
            }

            foreach (Text text in texts) {
                text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, 0), t / maxTime);
            }

            foreach (TMP_Text text in t_texts) {
                text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, 0), t / maxTime);
            }

            yield return null;
        }

        container.SetActive(false);


        for (int i = 0; i < images.Length; i++) {
            images[i].color = starterColorImages[i];
        }

        for (int i = 0; i < texts.Length; i++) {
            texts[i].color = starterColorTexts[i];
        }

    }
}
