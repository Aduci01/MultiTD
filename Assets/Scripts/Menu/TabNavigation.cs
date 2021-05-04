using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TabNavigation : MonoBehaviour {
    public List<TMP_InputField> fields;
    int fieldIndexer;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            fieldIndexer++;
            fields[fieldIndexer % fields.Count].Select();
        }
    }
}
