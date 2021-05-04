using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        transform.localPosition = new Vector3(-1f, 0.5f, 11f);
        transform.localScale = Vector3.one * 1f;
    }

    // Update is called once per frame
    void Update() {

    }
}
