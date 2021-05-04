using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {
    [HideInInspector] public Entity target;

    [Header("General")]
    public ShootEffect shootEffect;
    public bool attachToTransform;

    [HideInInspector] public DamageType damageType;

    [HideInInspector] public Stats stats;
    private float atkCountdown = 0f;

    public Transform partToRotate;
    public float turnSpeed = 60f;

    public Transform firePoint;

    [HideInInspector]
    public bool isShooting;

    Entity entity;

    ObjectPool pool;

    private void Start() {
        entity = GetComponent<Entity>();

        if (shootEffect != null) {
            pool = gameObject.AddComponent<ObjectPool>();
            pool.InitPool(shootEffect.gameObject, 0, attachToTransform ? firePoint : null);
        }
    }

    // Update is called once per frame
    void Update() {
        if (target == null || stats.range < Vector3.Distance(target.transform.position, transform.position)) {
            isShooting = false;
            return;
        }

        LockOnTarget();


        if (atkCountdown <= 0f) {
            Shoot();

            atkCountdown = stats.atkSpeed;
            isShooting = true;
        }

        atkCountdown -= Time.deltaTime;

    }

    void LockOnTarget() {
        if (partToRotate == null) return;

        Vector3 dir = target.transform.position - transform.position;

        float angle = Mathf.Lerp(transform.localEulerAngles.y, Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg, turnSpeed * Time.deltaTime);

        partToRotate.localEulerAngles = new Vector3(0, angle, 0);
    }

    void Shoot() {
        if (entity.animator != null)
            entity.animator.Play("Fight");

        if (shootEffect != null && RtsCamera._instance.IsVisible(firePoint.position)) {

            var s = pool.GetObjectFromPool().GetComponent<ShootEffect>();
            s.target = target.transform;

            s.transform.position = firePoint.position;
            s.gameObject.SetActive(true);
        }

        if (stats.splashRadius > 0) {
            DoSplashDamage();
        } else {
            target.TakeDamage(stats.damage, damageType);
        }

        if (stats.slowMovement != 0) {
            target.AddSlow(stats.slowMovement, 1.5f);
        }
    }

    void DoSplashDamage() {
        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, stats.splashRadius);
        foreach (var hitCollider in hitColliders) {
            Entity e = hitCollider.GetComponent<Entity>();

            if (e != null)
                e.TakeDamage(stats.damage, damageType);
        }
    }
}
