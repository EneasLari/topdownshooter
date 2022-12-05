using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public Transform muzzle;
    public Projectile projectile;
    public float msBetweebShots = 100;
    public float muzzlevelocity = 35;

    public Transform shell;
    public Transform shellEjection;
    MuzzleFlash muzzleFlash;

    float nextShotTime;

    private void Start()
    {
          muzzleFlash = GetComponent<MuzzleFlash>();
    }
    public void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweebShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(muzzlevelocity);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
        }

    }
}
