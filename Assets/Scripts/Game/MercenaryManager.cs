using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MercenaryManager : MonoBehaviour {
    public static MercenaryManager _instance;

    [SerializeField] GameObject window;

    [SerializeField] Transform mercenaryContainer;
    [SerializeField] MercenaryUiEntity mercenaryPrefab;

    List<MercenaryUiEntity> mercenaryList;

    private int currentMercs;
    private int maxMercenariesToBuy = 10;
    [SerializeField] TMP_Text limitText;
    [SerializeField] Image fillImage;

    private void Awake() {
        _instance = this;
    }

    private void Update() {
        if (!Tools.IsPointerOverUIObject() && Input.GetMouseButtonDown(0))
            window.SetActive(false);
    }

    // Start is called before the first frame update
    void Start() {
        mercenaryList = new List<MercenaryUiEntity>();

        foreach (EnemyData ed in DataCollection.enemyDb.Values.OrderBy(x => x.stats.levels[0].price)) {
            if (ed.stats.isPurchasable) {
                AddMercenary(ed);
            }
        }
    }

    /// <summary>
    /// Instantiating and Adding new mercenary to the ui
    /// </summary>
    /// <param name="data"></param>
    void AddMercenary(EnemyData data) {
        var m = Instantiate(mercenaryPrefab, mercenaryContainer);
        m.SetUp(data);

        m.gameObject.SetActive(true);

        mercenaryList.Add(m);
    }

    public void ShowWindow(bool b) {
        window.SetActive(b);
    }

    public void SetLimitUi(int current) {
        currentMercs = 0;

        limitText.text = current + "/" + maxMercenariesToBuy;
        fillImage.fillAmount = current / (float)maxMercenariesToBuy;
    }

    public void AddLimitUi(int n) {
        currentMercs += n;

        limitText.text = currentMercs + "/" + maxMercenariesToBuy;
        fillImage.fillAmount = currentMercs / (float)maxMercenariesToBuy;
    }

    public bool IsLimitReached() {
        if (currentMercs >= maxMercenariesToBuy) return true;
        else return false;
    }
}
