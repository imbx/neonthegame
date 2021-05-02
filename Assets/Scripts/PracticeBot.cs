using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BotStatus{
    NONE,
    SEEKINGWEAPON,
    SEEKINGPLAYER,
    TARGETINGPLAYER,
    DEAD
}

public class PracticeBot : MonoBehaviour
{
    private bool _isDead = false;
    public bool IsDead{ get {return _isDead;} }

    /*      PLAYER UNITY VARIABLES      */
    private Rigidbody2D _playerRB;
    private AudioSource audioSource;
    private BoxCollider2D bc;

    /*      OUR PLAYER VARIABLES      */
    public Weapon _weapon;
    private Facing _facing;
    private float _fireStartTimer = 0.5f;
    private float _fireCountTimer = 0f;


    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        bc = GetComponent<BoxCollider2D>();
    }

    public void Create(Weapon wp){
        _facing = Facing.None;
        _playerRB = GetComponent<Rigidbody2D>();
        
        Gradient trailGrad = transform.Find("Image").Find("Trail").GetComponent<TrailRenderer>().colorGradient;
        GradientColorKey[] cK = trailGrad.colorKeys;

        transform.Find("Image").GetComponent<SpriteRenderer>().color = ColorsExtension.Yellow;
        transform.Find("Image").GetComponent<SpriteRenderer>().material.SetColor("_BaseColor", ColorsExtension.Yellow);
        transform.Find("Image").GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", (Color)ColorsExtension.Yellow * Mathf.Pow(2f, 1f));
        cK[0].color = ColorsExtension.Yellow;
        
        trailGrad.colorKeys = cK;
        transform.Find("Image").Find("Trail").GetComponent<TrailRenderer>().colorGradient = trailGrad;
        gameObject.AddComponent<Weapon>();
        _weapon = GetComponent<Weapon>();
        _weapon.Copy(wp);
        _weapon.ownerIsBot = true;
        ShowWeapon(_weapon.weaponType);
    }

    void Update()
    {
        /*  FIRE INPUT  */
        if(_weapon){
            if (CheckFire()){
                _fireCountTimer += Time.deltaTime;

                if(_weapon.CanFire && _fireCountTimer >= _fireStartTimer){
                    _fireCountTimer = 0;
                    _weapon.Fire(_facing);
                }
            }
        }

        if(_facing == Facing.Right) transform.Find("Image").Find("Eyes").localPosition = (Vector3.right * 0.1f) + (Vector3.down * 0.15f);
        else if(_facing == Facing.Left) transform.Find("Image").Find("Eyes").localPosition = (Vector3.left * 0.1f) + (Vector3.down * 0.15f);
    }
    private bool CheckFire(){
        PlayerController[] pc = FindObjectsOfType<PlayerController>();
        if(pc.Length > 0){
            Vector3 playerPos = pc[0].transform.position;
            if(playerPos.x < transform.position.x){
                _facing = Facing.Left;
            } else _facing = Facing.Right;

            if(transform.position.y + 0.1f >= playerPos.y && transform.position.y - 0.1f <= playerPos.y){
                RaycastHit2D hit = Physics2D.Raycast((_facing == Facing.Left) ? transform.position + (Vector3.left * 0.16f): transform.position + (Vector3.right * 0.16f), (_facing == Facing.Left) ? Vector3.left : Vector3.right);
                if(hit.collider != null){
                    if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                        return false;
                    }
                }
                if(_weapon.weaponSettings.projectileSettings._maxDistance > 0){
                    if((playerPos - transform.position).magnitude <= _weapon.weaponSettings.projectileSettings._maxDistance){
                        return true;
                    }
                } else return true;  
            }
        }
        

        return false;
    }

    private void Die()
    {
        _isDead = true;
        FindObjectsOfType<PracticeLoader>()[0].AddKill();
        transform.Find("Image").gameObject.SetActive(false);
        bc.enabled = false;
        GetComponent<Explode>()._Explode = true;
        Destroy(gameObject, 0.5f);
    }

    private void ShowWeapon(WeaponType wt, bool toDestroy = false){
        string textMeshString = (wt == WeaponType.Gun) ? "GunText" : (wt == WeaponType.ShotGun) ? "ShotGunText" : (wt == WeaponType.Sniper) ? "SniperText" : (wt == WeaponType.Grenade) ? "GrenadeText" : (wt == WeaponType.Melee) ? "MeleeText" : "None";
        for(int i = 0 ; i < transform.childCount; i++){
            if(textMeshString ==  transform.GetChild(i).name || transform.GetChild(i).name == "Feet") transform.GetChild(i).gameObject.SetActive(!toDestroy);
            else if(transform.GetChild(i).name != "Image" && transform.GetChild(i).name != "SFXWeapon" && transform.GetChild(i).name != "Feet") transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.gameObject.tag == "DeathZone" || collision.gameObject.tag == "Bullet")
        {
            Die();
        }
    }
    private void OnCollisionStay2D(Collision2D other) {
        if(other.transform.GetComponent<BlockController>()){
            BlockController bc = other.transform.GetComponent<BlockController>();
            if(other.gameObject.layer == LayerMask.NameToLayer("Ground") && bc.movementBlock){
                transform.SetParent(other.transform);
            }
        }

    }
    private void OnCollisionExit2D(Collision2D other) {
        if(other.transform.GetComponent<BlockController>()){
            BlockController bc = other.transform.GetComponent<BlockController>();
            if(other.gameObject.layer == LayerMask.NameToLayer("Ground") && bc.movementBlock){
                transform.SetParent(null);
            }
        }
    }
}

