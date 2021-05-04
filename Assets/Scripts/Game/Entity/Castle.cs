using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour {
    public SkinnedMeshRenderer[] flags;

    public Transform mercenaryLayout;
    public MercenaryUiEntity mercenaryPrefab;

    // Start is called before the first frame update
    void Start() {
        transform.localPosition = new Vector3(-0.5f, 0.5f, -2.3f);
        transform.localScale = Vector3.one * 0.7f;
    }

    /// <summary>
    /// Setting the color of flags on castle - design purpose
    /// </summary>
    /// <param name="c"></param>
    public void SetFlagColor(Color c) {
        foreach (SkinnedMeshRenderer sm in flags) {
            sm.material.color = c;
        }
    }

    /// <summary>
    /// Clicking on Castle
    /// </summary>
    private void OnMouseUpAsButton() {
        if (!Tools.IsPointerOverUIObject())
            MercenaryManager._instance.ShowWindow(true);
    }

    /// <summary>
    /// Adding Mercenary icons to castle
    /// </summary>
    /// <param name="enemyId"></param>
    public void AddMercenary(EnemyData enemy) {
        var m = Instantiate(mercenaryPrefab, mercenaryLayout);
        m.SetUp(enemy);

        m.gameObject.SetActive(true);
    }

    /// <summary>
    /// Clearing all mercenary icons from castle
    /// </summary>
    public void ResetMercenaries() {
        foreach (MercenaryUiEntity m in mercenaryLayout.GetComponentsInChildren<MercenaryUiEntity>()) {
            Destroy(m.gameObject);
        }
    }
}
