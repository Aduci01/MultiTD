using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour {
    public Ground groundPrefab;
    public Material m1, m2;

    public int sizeX, sizeY;
    public float scale;

    public Ground[,] grounds;

    public void Init(bool b) {
        grounds = new Ground[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                var g = Instantiate(groundPrefab, transform);
                g.isLocal = b;

                g.transform.localPosition = new Vector3(i * scale + -sizeX / 2, 0, j * scale + -sizeY / 2 + 1);
                g.transform.localScale = Vector3.one * scale;

                grounds[i, j] = g;

                if ((i + j) % 2 == 0) {
                    g.SetMaterial(m2);
                } else g.SetMaterial(m1);
            }
        }
    }
}
