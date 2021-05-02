using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponSettings", menuName = "ScriptableObjects/WeaponSettings", order = 1)]
public class WeaponSettings : ScriptableObject
{
    public GameObject bulletPrefab;
    public int bulletNumber = 0;
    public Vector3[] spawnDirection;
    public float fireRate = 0.25f;
    public ProjectileSettings projectileSettings;
    public AudioClip fireSound;
}
