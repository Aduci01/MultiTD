using System;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class ChestOpening : MonoBehaviour {
    public static ChestOpening _instance;

    public ChestSettings chestSettings;

    [SerializeField] GameObject chestWindow;
    [SerializeField] Image chestImage;
    [SerializeField] GameObject coinObject;
    [SerializeField] GameObject clickVfx;

    [SerializeField] Transform collectedItemsLayout;

    ChestData chestData;

    int nextItemToCollectId;
    GameObject currentItem;
    bool readyToCollectNextItem;

    public bool chestReadyToOpen = true;

    private void Awake() {
        _instance = this;
    }

    public void OpenBronzeChest() {
        if (!chestReadyToOpen) return;
        chestReadyToOpen = false;

        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest() {
            FunctionName = "openBronzeChest",
            GeneratePlayStreamEvent = true,
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnClaimResult, OnPlayFabError);

        SetBronzeChest();
    }

    public void OpenSilverChest() {
        if (!chestReadyToOpen) return;
        chestReadyToOpen = false;

        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest() {
            FunctionName = "openSilverChest",
            GeneratePlayStreamEvent = true,
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnClaimResult, OnPlayFabError);

        SetSilverChest();
    }

    bool isTutorialChest;
    public void OpenTutorialChest() {
        if (!chestReadyToOpen) return;
        chestReadyToOpen = false;

        isTutorialChest = true;

        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest() {
            FunctionName = "openTutorialChest",
            GeneratePlayStreamEvent = true,
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnClaimResult, OnPlayFabError);

        SetSilverChest();
    }


    public void OnClaimResult(ExecuteCloudScriptResult result) {
        Debug.Log(result.FunctionResult.ToString());

        chestData = JsonUtility.FromJson<ChestData>(result.FunctionResult.ToString());

        chestWindow.SetActive(true);
        nextItemToCollectId = 0;

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result2 => {
            CurrencyManager._instance.SetCoins(result2.VirtualCurrency["CO"]);
            CurrencyManager._instance.SetCrystals(result2.VirtualCurrency["CR"]);
        }, error => {
            Debug.LogError(error.Error);
        });

        StartCoroutine(CollectItem());
    }

    IEnumerator CollectItem() {
        readyToCollectNextItem = false;
        Vector3 endScale = Vector3.one;

        if (nextItemToCollectId < chestData.units.Length) {
            currentItem = GetUnit(chestData.units[nextItemToCollectId]);

            currentItem.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 425);

            endScale = Vector3.one * 0.8f;
        } else {
            currentItem = Instantiate(coinObject, chestWindow.transform);
            currentItem.GetComponentInChildren<TMPro.TMP_Text>().text = "x" + chestData.coins;
        }

        currentItem.SetActive(true);

        float t = 0;
        while (t < 1) {
            t += Time.deltaTime * 2f;

            currentItem.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(-330, 100), t);
            currentItem.transform.localScale = Vector3.Lerp(Vector3.zero, endScale, t);

            yield return null;
        }


        nextItemToCollectId++;
        readyToCollectNextItem = true;
    }

    GameObject GetUnit(string id) {
        /*CardCollection._instance.AddUsagesToUnit(id, chestData.amounts[nextItemToCollectId]);

        var g = Instantiate(CardCollection._instance.unitsList[id].gameObject, chestWindow.transform);
        g.GetComponent<Card>().stackAmountText.text += "  +" + chestData.amounts[nextItemToCollectId] + "x";

        g.GetComponent<Button>().interactable = false;

        return g;*/
        return null;
    }

    public void OnClickChest() {
        if (!readyToCollectNextItem) return;

        currentItem.transform.SetParent(collectedItemsLayout);
        //currentItem.transform.localScale = Vector3.one * 1.2f;

        if (nextItemToCollectId <= chestData.units.Length)
            StartCoroutine(CollectItem());
        else {

            CloseWindow();
        }
    }

    void CloseWindow() {
        chestReadyToOpen = true;

        chestWindow.SetActive(false);

        foreach (Transform t in collectedItemsLayout.GetComponentInChildren<Transform>()) {
            if (t.parent = collectedItemsLayout)
                Destroy(t.gameObject);
        }

        UI_VFX_FromTo._instance.CoinIncrease();
    }

    private void OnPlayFabError(PlayFabError obj) {
        Debug.LogError(obj.GenerateErrorReport());
    }

    public void SetBronzeChest() {
        chestImage.sprite = chestSettings.bronzeChest;
    }

    public void SetSilverChest() {
        chestImage.sprite = chestSettings.silverChest;
    }

    public void SetGoldChest() {
        chestImage.sprite = chestSettings.goldChest;
    }

    public void SetPlatinumChest() {
        chestImage.sprite = chestSettings.platinumChest;
    }
}

[Serializable]
public class ChestData {
    public bool success;
    public string[] units;
    public int[] amounts;
    public int coins;
}

[Serializable]
public class ChestSettings {
    public Sprite bronzeChest, silverChest, goldChest, platinumChest;
    public Sprite bronzeChestLocked, silverChestLocked, goldChestLocked, platinumChestLocked;
}