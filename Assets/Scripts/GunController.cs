using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun startingGun;

    Gun equipedGun;

    void Start()
    {
        if (startingGun != null)
        {
            EquipGun(startingGun);
        }
    }
    public void EquipGun(Gun gunToEquip)
    {
        if (equipedGun != null)
        {
            Destroy(equipedGun.gameObject);
        }
        equipedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equipedGun.transform.parent = weaponHold;
    }

    public void OnTriggerHold()
    {
        if (equipedGun != null)
        {
            equipedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equipedGun != null)
        {
            equipedGun.OnTriggerRelease();
        }
    }

    public void Aim(Vector3 aimPoint)
    {
        if (equipedGun != null)
        {
            equipedGun.Aim(aimPoint);
        }
    }

    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }
}
