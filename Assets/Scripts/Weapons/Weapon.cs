using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    public WeaponType weaponType;
    public WeaponSettings weaponSettings;
    private int _bulletCounter = 0;
    private float _fireTimer = 0f;
    private AudioSource audioSource;
    public float spawnOffset = 0.15f;
    public bool ownerIsBot = false;

    public bool Fire(Facing f){
        if(_bulletCounter > 0 && CanFire){
            _fireTimer = 0f;
            if(audioSource)
                audioSource.Play();
            foreach(Vector3 v in weaponSettings.spawnDirection){
                GameObject b = Instantiate(weaponSettings.bulletPrefab);
                b.transform.SetParent(GameManager.Instance.blackboard.BulletContainer);
                bool isFacingRight = f == Facing.Right;
                b.transform.position = transform.position + (((isFacingRight) ? 1 :- 1) * (v * spawnOffset));
                b.GetComponent<Projectile>().SetData(weaponSettings.projectileSettings, v * ((isFacingRight) ? 1 : -1));
            }
            if(!ownerIsBot)_bulletCounter--;
            return HasBullets;
        }
        else return false;
    }

    public bool CanFire{ get { return (_fireTimer >= weaponSettings.fireRate) ? true : false; } }

    public bool HasBullets{ get { return (_bulletCounter > 0) ? true : false; } }

    void Update(){ _fireTimer += Time.deltaTime; }

    public void Copy(Weapon w){
        weaponType = w.weaponType;
        weaponSettings = w.weaponSettings;
        _bulletCounter = weaponSettings.bulletNumber;
        _fireTimer = weaponSettings.fireRate;

        audioSource = transform.Find("SFXWeapon").GetComponent<AudioSource>();
        if(audioSource) audioSource.clip = weaponSettings.fireSound;
    }
}
