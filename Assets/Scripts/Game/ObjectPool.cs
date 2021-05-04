using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    public List<GameObject> poolObjects;
    private GameObject objectToPool;

    Transform parentTransform;

    public void InitPool(GameObject objToPool, int n, Transform t) {
        objectToPool = objToPool;
        poolObjects = new List<GameObject>();
        parentTransform = t;

        for (int i = 0; i < n; i++) {
            InstatiateObject(objectToPool, parentTransform);
        }
    }

    public GameObject GetObjectFromPool() {
        foreach (GameObject g in poolObjects) {
            if (g == null || g.activeInHierarchy) continue;

            return g;
        }

        return InstatiateObject(objectToPool, parentTransform);
    }

    /// <summary>
    /// Instatiating gameobject and adding it to the pool
    /// </summary>
    /// <param name="g"></param>
    /// <returns></returns>
    GameObject InstatiateObject(GameObject g, Transform t) {
        var obj = Instantiate(g, t);
        obj.SetActive(false);
        poolObjects.Add(obj);

        return obj;
    }

    private void OnDestroy() {
        foreach (GameObject g in poolObjects) {
            Destroy(g);
        }
    }
}
