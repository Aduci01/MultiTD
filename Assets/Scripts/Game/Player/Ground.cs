using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {
    public GameObject[] decors;
    GameObject decor;

    public MeshRenderer meshRenderer;

    public int decorChance = 5;
    public float decorScale = 0.3f;
    public float decorOffset;

    public bool isLocal;

    // Start is called before the first frame update
    void Start() {
        if (Random.Range(0, 100) < decorChance) {
            decor = Instantiate(decors[Random.Range(0, decors.Length)], transform);
            decor.transform.localPosition = new Vector3(decorOffset, 0.5f, decorOffset);
            decor.transform.localScale = Vector3.one * decorScale;
        }
    }

    public void SetMaterial(Material m) {
        meshRenderer.material = m;
    }
}
