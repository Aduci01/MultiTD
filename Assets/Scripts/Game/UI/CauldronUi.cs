using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CauldronUi : MonoBehaviour {
    public static CauldronUi _instance;
    public GameObject window;

    Cauldron currentCauldronBuilding;

    public Slider amountSlider;
    public TMP_Text manaText, goldText;
    public Text ratioText, limitText;

    private void Awake() {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start() {

    }

    public void SetupWindow(Cauldron cauldron) {
        currentCauldronBuilding = cauldron;
        SetStaticTexts();

        SetValues(0);

        window.SetActive(true);
    }

    private void Update() {
        if (!Tools.IsPointerOverUIObject() && Input.GetMouseButtonDown(0))
            window.SetActive(false);
    }

    public void OnSliderChange() {
        SetValues(amountSlider.value);
    }

    /// <summary>
    /// OnClick convert button, sending request to server
    /// </summary>
    public void ConvertMana() {
        ClientSend.CauldronEvent(currentCauldronBuilding.building, currentCauldronBuilding.amount);

        if (currentCauldronBuilding.amount > GameManager._instance.localPlayer.manaCurrency) return;

        currentCauldronBuilding.manaConverted += currentCauldronBuilding.amount;

        amountSlider.value = 0;
        SetStaticTexts();
        SetValues(0);
    }

    void SetValues(float sliderValue) {
        currentCauldronBuilding.amount = (int)Mathf.Lerp(0, currentCauldronBuilding.maxManaConverted - currentCauldronBuilding.manaConverted, sliderValue);

        manaText.text = currentCauldronBuilding.amount + "";
        goldText.text = (int)(currentCauldronBuilding.amount / currentCauldronBuilding.convertRatio) + "";

        if (currentCauldronBuilding.amount > GameManager._instance.localPlayer.manaCurrency) {
            manaText.color = Color.red;
            goldText.color = Color.red;
        } else {
            manaText.color = Color.magenta;
            goldText.color = Color.yellow;
        }
    }

    void SetStaticTexts() {
        limitText.text = "Limit: " + currentCauldronBuilding.manaConverted + "/" + currentCauldronBuilding.maxManaConverted;
        ratioText.text = "1 : " + currentCauldronBuilding.convertRatio;
    }
}
