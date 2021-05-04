using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour {
    public Canvas canvas;

    public float scaleFactor;
    public Transform border;

    Vector2 startPos;

    private void Start() {
        startPos = border.localPosition;
    }

    private void Update() {
        Vector3 v = RtsCamera._instance.transform.localPosition * scaleFactor;
        border.localPosition = new Vector2(v.x + startPos.x, v.z + startPos.y);
    }

    public void OnPointerDrag() {
        Vector2 pos = Tools.ScreenToCanvasPosition(canvas, Input.mousePosition);
        border.position = pos;

        Vector2 rPos = border.localPosition + Vector3.left * startPos.x + Vector3.down * startPos.y;
        rPos /= scaleFactor;

        RtsCamera._instance.transform.localPosition = new Vector3(rPos.x, RtsCamera._instance.transform.localPosition.y, rPos.y);
    }
}
