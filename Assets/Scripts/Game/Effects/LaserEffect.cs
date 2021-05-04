using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEffect : MonoBehaviour {
    public Shooter shooter;

    public GameObject start;
    public Transform end;
    public LineRenderer middle;

    // Start is called before the first frame update
    void Start() {
        middle.SetPosition(0, shooter.firePoint.position);
    }

    // Update is called once per frame
    void Update() {
        if (!shooter.isShooting) {
            Activate(false);
            return;
        }

        Activate(true);

        end.position = shooter.target.transform.position;
        middle.SetPosition(0, shooter.firePoint.position);
        middle.SetPosition(1, end.position);
    }

    void Activate(bool b) {
        if (start.activeSelf == b) return;

        start.SetActive(b);
        end.gameObject.SetActive(b);
        middle.gameObject.SetActive(b);
    }
}
