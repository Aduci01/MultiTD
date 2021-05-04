using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PurchasableIcon : MonoBehaviour {
    public Image icon;

    public abstract void OnClick();

    public abstract void PointerEnter();
    public abstract void PointerExit();

    protected GameObject InstatiateObject(GameObject g, PlayerManager pm, Vector3 offset) {
        var newG = Instantiate(g, pm.transform);

        Ground ground = pm.groundSpawner.grounds[pm.groundSpawner.sizeX / 2, pm.groundSpawner.sizeY / 2];
        newG.transform.position = ground.transform.position + offset;

        return newG;
    }
}
