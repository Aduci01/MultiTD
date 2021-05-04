using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Placeable : MonoBehaviour {
    public static Placeable currentPlaceable;

    public Vector3 offset;
    public SpriteRenderer validatorSprite;

    // Start is called before the first frame update
    void Start() {
        currentPlaceable = this;
    }

    // Update is called once per frame
    void Update() {
        CheckInvalidatePlacement();

        if (Tools.IsPointerOverUIObject()) return;

        Vector2 mousePos = Input.mousePosition;
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, RtsCamera._instance.offsetZ));
        transform.position = new Vector3(Mathf.RoundToInt(pos.x) - offset.x, transform.position.y, Mathf.RoundToInt(pos.z) - offset.z);

        bool isValid = IsPlaceValid();

        if (!isValid) {
            validatorSprite.color = new Color(1, 0, 0, 0.5f);
        } else validatorSprite.color = new Color(1, 1, 1, 0.5f);

        if (Input.GetMouseButtonDown(0)) {
            RequestPlacing(isValid);
        }
    }

    /// <summary>
    /// Request sending to server
    /// </summary>
    /// <param name="isValid"></param>
    void RequestPlacing(bool isValid) {
        if (isValid) {
            if (GetComponent<Unit>() != null)
                ClientSend.PlacementRequestUnit(GetComponent<Unit>().data, transform.localPosition);

            else if (GetComponent<Building>() != null)
                ClientSend.PlacementRequestBuilding(GetComponent<Building>().data, transform.localPosition);
        }

        Destroy();
    }

    /// <summary>
    /// Checking if there is any obstacle around the desired placement position
    /// </summary>
    /// <returns></returns>
    bool IsPlaceValid() {
        Collider[] cols = Physics.OverlapBox(transform.position, Vector3.one * .49f);

        bool isLocalGround = false;

        foreach (Collider c in cols) {
            Ground gr = c.GetComponent<Ground>();
            if (gr != null) {
                if (!c.GetComponent<Ground>().isLocal)
                    return false;
                else isLocalGround = true;
            } else if (c.gameObject != gameObject) return false;
        }

        return isLocalGround;
    }


    /// <summary>
    /// Destroys the current Placeable
    /// </summary>
    public void Destroy() {
        currentPlaceable = null;
        Destroy(gameObject);
    }

    /// <summary>
    /// Destroy current placeable in given input
    /// </summary>
    void CheckInvalidatePlacement() {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            Destroy();

        if (Tools.IsPointerOverUIObject() && Input.GetMouseButtonDown(0))
            Destroy();
    }
}
