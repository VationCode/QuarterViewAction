using System;
using UnityEngine;

[Serializable]
public struct RangeWeaponInfo
{
    public int currentAmmo;
    public int maxLoadAmmo;
    public int magazineAmmo;
    public int maxMagazineAmmo;
    public GameObject bullet;
    public Transform bulletPos;
    public GameObject bulletCase;
    public Transform bulletCasePos;
}
