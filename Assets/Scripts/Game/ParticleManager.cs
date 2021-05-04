using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParticleManager : MonoBehaviour {
    public static ParticleManager _instance;

    public GameObject goldGained;
    public GameObject entityPlaced;
    public GameObject unitPlaced;
    public GameObject sell;

    private void Awake() {
        _instance = this;
    }

    public void SpawnParticle(GameObject g, Transform t, Vector3 pos, Vector3 rotation) {
        if (g == null) return;
        if (!RtsCamera._instance.IsVisible(pos)) return;

        var particle = Instantiate(g, t);

        particle.transform.position = pos;
        particle.transform.eulerAngles = rotation;
    }
}
