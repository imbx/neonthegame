using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    /*      PLAYER MOST IMPORTANT DATA      */
    private int _index;
    public int Idx{ get { return _index; } }
    private bool _isDead = false;
    public bool IsDead{ get {return _isDead;} }

    /*      PLAYER UNITY VARIABLES      */
    private Rigidbody2D _playerRB;
    private InputsHandler Input;
    private Transform _feet;
    private AudioSource audioSource;
    private BoxCollider2D bc;

    /*      OUR PLAYER VARIABLES      */
    public PlayerSettings _p;
    public Weapon _weapon;
    private Facing _facing;
    private Vector3 _mov;

    private bool _isGround = true;
    private bool _isJumping;
    private bool _isSecondJump = false;
    private bool _canSecondJump = false;
    private bool _justPressedJump = false;
    private bool _jumpChecker = false;
    private bool _playerPressedJump = false;
    private float _jumpTimeCounter;
    private bool JumpAnimActive = false;
    private float lateralSize = 0.14f;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        bc = GetComponent<BoxCollider2D>();
        
    }

    public void Create(int id, InputsHandler ip){
        _index = id;
        Input = ip;

        _facing = Facing.None;
        _playerRB = GetComponent<Rigidbody2D>();
        
        GameObject GO_temp = new GameObject("Feet");
        
        _feet = GO_temp.transform;
        _feet.SetParent(transform);
        _feet.localPosition = Vector3.down * (transform.Find("Image").GetComponent<SpriteRenderer>().size.y * _p._distanceMultiplier);
        Gradient trailGrad = transform.Find("Image").Find("Trail").GetComponent<TrailRenderer>().colorGradient;
        GradientColorKey[] cK = trailGrad.colorKeys;
        Color playerColor = (GameManager.Instance.level) ? GameManager.Instance.blackboard.GetPlayerDataWithId(id).playerColor : (Color)ColorsExtension.Red;
        transform.Find("Image").GetComponent<SpriteRenderer>().color = playerColor;
        transform.Find("Image").GetComponent<SpriteRenderer>().material.SetColor("_BaseColor", playerColor);
        transform.Find("Image").GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", (Color)playerColor * Mathf.Pow(2f, _p._bloomBase));
        cK[0].color = playerColor;
        
        trailGrad.colorKeys = cK;
        transform.Find("Image").Find("Trail").GetComponent<TrailRenderer>().colorGradient = trailGrad;

    }
    void FixedUpdate()
    {   
        if(_mov.sqrMagnitude > 0)
        {
            _mov *= _p._easeMultiplier;
            if(_mov.magnitude < 0.01f) _mov = Vector3.zero;
        }

        _playerRB.velocity = new Vector2(_mov.x * GameManager.Instance.TimeScale, _playerRB.velocity.y);
    }

    void Update()
    {
        if(GameManager.Instance.TimeScale > 0f){
            /*  CHECK GROUND  */

            _isGround = Physics2D.OverlapCircle(_feet.position, _p._feetRadius, _p._ground);
            if(!_isGround) _isGround = Physics2D.OverlapCircle(_feet.position + (Vector3.left * lateralSize * 0.45f), _p._feetRadius, _p._ground) || Physics2D.OverlapCircle(_feet.position + (Vector3.right * lateralSize * 0.45f), _p._feetRadius, _p._ground);

            /*  VERTICAL MOVEMENT HAS TO BE FIXED  */

            if(Input.InputJump && !_justPressedJump){
                _justPressedJump = true;
                _jumpChecker = true;
            }
            else if(_jumpChecker) _jumpChecker = false;
        
            if(_jumpChecker && _isSecondJump && _canSecondJump){
                _canSecondJump = false;
                _isSecondJump = false;
                _playerPressedJump = true;
                StartCoroutine(JumpAnimation());
                _playerRB.velocity = Vector2.up * _p._jumpForce;
                _jumpTimeCounter = _p._jumpTime;
                _isJumping = true;
            }

            if(_isGround && _jumpChecker){
                StartCoroutine(JumpAnimation());
                _playerRB.velocity = Vector2.up * _p._jumpForce;
                _jumpTimeCounter = _p._jumpTime;
                _isJumping = true;
                _canSecondJump = true;
                _playerPressedJump = true;
            }

            if(!_isGround && !_isJumping && !_playerPressedJump)
            {
                _canSecondJump = true;
                _isSecondJump = true;
            }

            if(_isGround) _playerPressedJump = false;

            if(_jumpChecker && _isJumping){
                if(_jumpTimeCounter > 0){
                    _playerRB.velocity = Vector2.up * _p._jumpForce;
                    _jumpTimeCounter -= Time.deltaTime;
                }
                else _isJumping = false;
            }

            if(_isGround) _isSecondJump = false;

            if(!Input.InputJump && _justPressedJump) {
                if(_isJumping) _isSecondJump = true;
                _isJumping = false;
            }

            /*  HORIZONTAL MOVEMENT  */

            float xAxis =  Input.InputHoriz;

            if(xAxis > 0) _facing = Facing.Right;
            else if(xAxis < 0) _facing = Facing.Left;

            _mov = Vector3.right * xAxis * _p._moveSpeed;

            if(_facing == Facing.Right) transform.Find("Image").Find("Eyes").localPosition = Vector3.right * 0.025f;
            else if(_facing == Facing.Left) transform.Find("Image").Find("Eyes").localPosition = Vector3.left * 0.025f;
            
            /*  SHIFT MOVEMENT  */

            if(Input.InputShift)
            {
                transform.localScale = new Vector3(1f, 0.5f, 1f);
                transform.Find("Image").Find("Trail").GetComponent<TrailRenderer>().startWidth = _p._minTrailSize * 0.5f;
                _mov *= _p._shiftedMultiplier;
                _playerRB.velocity *= _p._shiftedMultiplier;
            }
            else 
            {
                transform.Find("Image").Find("Trail").GetComponent<TrailRenderer>().startWidth = _p._minTrailSize;
                transform.localScale = Vector3.one;
            }

            /*  FIRE INPUT  */
            if(_weapon){
                if (Input.InputFire){
                    if(_weapon.CanFire){
                        _weapon.Fire(_facing);
                    }
                }
                if(!_weapon.HasBullets && !_isDead) {
                    ShowWeapon(_weapon.weaponType, true);
                    Destroy(_weapon);
                }
            }

            /*  LAST JUMP  */

            if(!Input.InputJump && _justPressedJump)
            {
                _justPressedJump = false;
            }
        }
    }
    public IEnumerator JumpAnimation()
    {
        Vector2 originalSize = transform.Find("Image").gameObject.GetComponent<SpriteRenderer>().size;
        
        if (!JumpAnimActive)
        {
            JumpAnimActive = true;
            Vector2 transformedSize = originalSize;
            Vector2 maxSize = new Vector2(originalSize.x * 0.4f, _p._maxHeightExtension);
            bool firstGo = true;

            while ((transformedSize.y < _p._maxHeightExtension  && firstGo) || (!firstGo && transformedSize.y > originalSize.y))
            {
                transformedSize = (firstGo) ? Vector2.Lerp(transform.Find("Image").gameObject.GetComponent<SpriteRenderer>().size, maxSize, Time.deltaTime * _p._jumpAnimSpeed) : Vector2.Lerp(transform.Find("Image").gameObject.GetComponent<SpriteRenderer>().size, originalSize, Time.deltaTime * _p._jumpAnimSpeed);
                if ((transformedSize.y < (originalSize.y + 0.015f)) && !firstGo) transformedSize = originalSize;

                if (transformedSize.y >= (_p._maxHeightExtension - (transformedSize.y * 0.25f)) && firstGo)
                {
                    transformedSize = maxSize;
                    firstGo = false;
                }
                transform.Find("Image").gameObject.GetComponent<SpriteRenderer>().size = transformedSize;
                yield return null;
            }
            transform.Find("Image").gameObject.GetComponent<SpriteRenderer>().size = originalSize;
            JumpAnimActive = false;
            yield return null;
        }
        yield return null;
    }

    private void Die()
    {
        _isDead = true;
        transform.Find("Image").gameObject.SetActive(false);
        bc.enabled = false;
        GetComponent<Explode>()._Explode = true;
    }

    private void ShowWeapon(WeaponType wt, bool toDestroy = false){
        string textMeshString = (wt == WeaponType.Gun) ? "GunText" : (wt == WeaponType.ShotGun) ? "ShotGunText" : (wt == WeaponType.Sniper) ? "SniperText" : (wt == WeaponType.Grenade) ? "GrenadeText" : (wt == WeaponType.Melee) ? "MeleeText" : (wt == WeaponType.Ak) ? "RifleText" : (wt == WeaponType.Wasd) ? "WasdText" : (wt == WeaponType.Axis) ? "AxisText" : "None";
        for(int i = 0 ; i < transform.childCount; i++){
            if(textMeshString ==  transform.GetChild(i).name || transform.GetChild(i).name == "Feet") transform.GetChild(i).gameObject.SetActive(!toDestroy);
            else if(transform.GetChild(i).name != "Image" && transform.GetChild(i).name != "SFXWeapon" && transform.GetChild(i).name != "Feet") transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (collision.gameObject.tag == "Gun")
        {
            if(!GetComponent<Weapon>())
            {
                gameObject.AddComponent<Weapon>();
                _weapon = GetComponent<Weapon>();
            }
            
            _weapon.Copy(collision.gameObject.GetComponent<Weapon>());
            ShowWeapon(_weapon.weaponType);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "DeathZone" || collision.gameObject.tag == "Bullet")
        {
            Die();
        }
    }
    private void OnCollisionStay2D(Collision2D other) {
        if(other.transform.GetComponent<BlockController>()){
            BlockController bc = other.transform.GetComponent<BlockController>();
            if(other.gameObject.layer == LayerMask.NameToLayer("Ground") && bc.movementBlock){
                _isGround = true;
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
