using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PracticeLoader : MonoBehaviour
{
    private GameObject _PrefabBackground;
    private GameObject _PrefabTerrain;
    private GameObject _PrefabPlayer;
    private GameObject _PrefabUIPractice;
    private GameObject _PrefabInputPlayer;
    private GameObject _PrefabUIPracticeEnd;

    private GameObject _Background;
    private GameObject _UIPractice;
    private GameObject _UIEnd;
    private GameObject _Terrain;

    private PlayerSpawner[] _PlayerSpawner;
    private List<GameObject> _Players;
    private List<BlockController> Blocks;
    private GameObject _DeathZone;
    private GameObject _Bullets;
    public Transform BulletContainer{
        get{ return _Bullets.transform; }
    }

    private UIPlaying _UI;
    private bool Reset = false;
    private PlayerInputManager _playerInputManager;
    private LevelData CurrentData = default;
    [HideInInspector]
    public LevelState State = LevelState.FIRSTLOAD;
    public float secondsBetweenBlock = 0.1f;
    private Material _MatBlock;
    private Material _MatBeat;

    public float SlowmoMinScale = 0.25f;
    public float SlowmoMultiplier = 1f;

    private float _UITimer = 0f;
    private List<InputsHandler> _Inputs;
    private BotSpawner[] _BotSpawners;

    void Awake()
    {
        GameManager.Instance.pLevel = this;
        _PrefabBackground = Resources.Load<GameObject>("Prefabs/Level/Background");
        _PrefabTerrain = Resources.Load<GameObject>("Prefabs/Level/PracticeLevel");
        _PrefabPlayer = Resources.Load<GameObject>("Prefabs/PlayerPrefab");
        _PrefabInputPlayer = Resources.Load<GameObject>("Prefabs/PlayerInputs");
        _PrefabUIPractice = Resources.Load<GameObject>("Prefabs/UI/UIPractice");
        _PrefabUIPracticeEnd = Resources.Load<GameObject>("Prefabs/UI/UIPracticeEnd");

        _MatBlock = Resources.Load<Material>("Materials/BlockLevel/BlockMat");
        _MatBeat = Resources.Load<Material>("Materials/BlockLevel/BeatMat");

        _Background = Instantiate(_PrefabBackground);
        _UIPractice = Instantiate(_PrefabUIPractice);
        _UIEnd = Instantiate(_PrefabUIPracticeEnd);
        _Inputs = new List<InputsHandler>();
        State = LevelState.FIRSTLOAD;

        _UI = _UIPractice.GetComponent<UIPlaying>();

        _DeathZone = new GameObject("DeathZone", typeof(BoxCollider2D));
        _DeathZone.tag = "DeathZone";
        _DeathZone.transform.position = Vector3.down * 2f;
        _DeathZone.transform.GetComponent<BoxCollider2D>().isTrigger = true;
        _DeathZone.transform.GetComponent<BoxCollider2D>().size = new Vector2(14f, 1f);
    }

    void OnEnable()
    {
        StartCoroutine(LevelUpdate());
    }

    void OnDisable(){
        StopAllCoroutines();
    }

    public IEnumerator LevelUpdate(){
        while(GameManager.Instance.CurrentState() == StateType.PRACTICE || GameManager.Instance.CurrentState() == StateType.ENDPRACTICE){
            switch(State){    
                case LevelState.FIRSTLOAD:
                {
                    for(int i = 0; i < GameManager.Instance.blackboard.players; i++){
                        InputsHandler ih = default;
                        if(i == 0) ih = PlayerInput.Instantiate(_PrefabInputPlayer, controlScheme: "WASD", pairWithDevice: Keyboard.current).transform.GetComponent<InputsHandler>();
                        else if(i == 1) ih = PlayerInput.Instantiate(_PrefabInputPlayer, controlScheme: "Arrows", pairWithDevice: Keyboard.current).transform.GetComponent<InputsHandler>();
                        _Inputs.Add(ih);
                    }
                    State = LevelState.LOADINGDATA;
                    break;
                }            
                case LevelState.LOADINGDATA:
                {
                    Blocks = new List<BlockController>(); 
                    _Terrain = Instantiate(_PrefabTerrain);
                    
                    CurrentData = _Terrain.GetComponent<LevelData>();
                    _MatBlock.SetColor("_BaseColor", Color.black);
                    _MatBeat.SetColor("_EmissionColor", CurrentData._ScenaryColor);
                    
                    if(_Terrain.transform.Find("Blocks"))
                    {
                        Blocks.AddRange(_Terrain.transform.Find("Blocks").GetComponentsInChildren<BlockController>());
                        Blocks.Shuffle();
                    }

                    _Bullets = new GameObject("Bullets");
                    GameManager.Instance.blackboard.BulletContainer = _Bullets.transform;
                    _Players = new List<GameObject>();
                    _playerInputManager = GetComponent<PlayerInputManager>();

                    _PlayerSpawner = _Terrain.GetComponentsInChildren<PlayerSpawner>();
                    _BotSpawners = _Terrain.GetComponentsInChildren<BotSpawner>();

                    State = LevelState.UPDATINGBACKGROUND;
                    break;
                }
                case LevelState.UPDATINGBACKGROUND:
                {

                    Gradient meshCurrentGradient = _Background.GetComponent<MeshGenerator>()._lineGradient;
                    GradientColorKey[] bgck = CurrentData._BackgroundGradient.colorKeys;
                    int nextNumKeys = bgck.Length;

                    GradientColorKey[] gck = new GradientColorKey[nextNumKeys];
                    GradientAlphaKey[] gak = meshCurrentGradient.alphaKeys;

                    FindObjectsOfType<AudioPeer>()[0].SetMatColor(CurrentData._BarsMainColor, CurrentData._BarsSecondColor);

                    for(int i = 0; i < nextNumKeys; i++)
                    {
                        gck[i] = new GradientColorKey(meshCurrentGradient.Evaluate(bgck[i].time), bgck[i].time);
                        yield return null;
                    }

                    meshCurrentGradient.SetKeys(gck, gak);

                    while(gck[0].color != bgck[0].color){
                        for(int i = 0; i < nextNumKeys; i++)
                        {
                            Color mcg = gck[i].color;
                            mcg = Color.Lerp(mcg, bgck[i].color, Time.deltaTime * 35f);
                            gck[i] = new GradientColorKey(mcg, gck[i].time);
                            yield return null;
                        }
                        
                        meshCurrentGradient.SetKeys(gck, gak);

                        _Background.GetComponent<MeshGenerator>().SetGradient(meshCurrentGradient);

                        yield return new WaitForSeconds(secondsBetweenBlock * 0.5f);
                    }
                    _Background.GetComponent<MeshGenerator>().SetGradient(CurrentData._BackgroundGradient);
                    State = LevelState.LOADINGLEVEL;
                    break;
                }
                case LevelState.LOADINGLEVEL:
                {
                    foreach(BlockController bc in Blocks) {
                        bc.Move();
                        yield return new WaitForSeconds(secondsBetweenBlock);
                    }

                    while(Blocks[Blocks.Count - 1].HasToMove || Blocks[Blocks.Count - 1].IsOut){
                        yield return null;
                    }

                    State = LevelState.LOADINGPLAYERS;
                    break;
                }
                case LevelState.LOADINGPLAYERS:
                {
                    Color tempColor = Color.black;
                    while(tempColor != CurrentData._ScenaryColor){
                        tempColor = Color.Lerp(tempColor, CurrentData._ScenaryColor, Time.deltaTime * 10f);
                        _MatBeat.SetColor("_EmissionColor", tempColor * Mathf.Pow(2f, AudioPeer._AmplitudeBuffer));
                        yield return null;
                    }
                    int startPlayerSpawner = Random.Range(0, _PlayerSpawner.Length - 1);

                    GameObject go = Instantiate(_PrefabPlayer);
                    go.transform.position = _PlayerSpawner[startPlayerSpawner].GetPosition();
                    PlayerController pc = go.GetComponent<PlayerController>();
                    pc.Create(0, _Inputs[0]);
                    _Players.Add(go);

                    State = LevelState.PLAYING;
                    break;
                }
                case LevelState.PLAYING:
                {
                    _MatBeat.SetColor("_EmissionColor", CurrentData._ScenaryColor * Mathf.Pow(2f, AudioPeer._AmplitudeBuffer));

                    foreach(GameObject pl in _Players){
                        if( pl.GetComponent<PlayerController>()){
                            PlayerController pc = pl.GetComponent<PlayerController>();
                            if(pc.IsDead){
                                _Players.Remove(pl);
                                Destroy(pl);
                                _UI.RemPoint(0);
                                Reset = true;
                                break;
                            }
                        }
                        yield return null;
                    }

                    if(Reset){
                        Reset = false;
                        State = LevelState.LOADINGPLAYERS;                        
                    }
    
                    break;
                }
                case LevelState.REMOVINGLEVEL:
                {
                    foreach(GameObject go in _Players) go.SetActive(false);

                    for(int i = 0; i < Blocks.Count; i++){
                        Blocks[i].Move();
                        yield return new WaitForSeconds(secondsBetweenBlock);
                    }

                    while(Blocks[Blocks.Count - 1].transform.position.y > -3f){
                        yield return null;
                    }
                    
                    State = LevelState.RESETINGLEVEL;
                    break;
                }
                case LevelState.RESETINGLEVEL:
                {
                    foreach(GameObject go in _Players) Destroy(go);
                    if(_Terrain) Destroy(_Terrain);
                    if(_Bullets) Destroy(_Bullets);
                    Blocks.Clear();
                    State = LevelState.SEEDING;
                    break;
                }
                case LevelState.ENDGAME:
                {
                    while(GameManager.Instance.TimeScale > SlowmoMinScale){
                        GameManager.Instance.TimeScale -= Time.deltaTime * SlowmoMultiplier;
                        yield return null;
                    }
                    
                    GameManager.Instance.TimeScale = SlowmoMinScale;

                    Material winnerBg = _UIEnd.transform.Find("Background").GetComponent<UnityEngine.UI.Image>().material;
                    float blurVal = 0;
                    winnerBg.SetFloat("_BlurValue", blurVal);
                    _UIEnd.transform.Find("Background").gameObject.SetActive(true);

                    while(winnerBg.GetFloat("_BlurValue") < 0.002f){
                        blurVal += Time.deltaTime * 0.025f;
                        winnerBg.SetFloat("_BlurValue", blurVal);
                        yield return null; 
                    }

                    winnerBg.SetFloat("_BlurValue", 0.002f);

                    string playerWinner = "YOU GOT " + GameManager.Instance.blackboard.Player1Score + " POINT/S\n IN " + GameManager.Instance.blackboard.timerPractice + " SECONDS";
                    _UIEnd.transform.Find("Winner").GetComponent<UnityEngine.UI.Text>().text = playerWinner;
                    _UIEnd.transform.Find("Winner").gameObject.SetActive(true);

                    yield return new WaitForSeconds(2.5f * SlowmoMinScale);

                    while(!Input.anyKey){
                        yield return null;
                    }

                    GameManager.Instance.CurrentState(StateType.MENU);
                    break;
                }
                case LevelState.WAITING:
                {
                    break;
                }
            }

            yield return null;
        }
        yield return null;
    }
    void Update(){
        if(State == LevelState.PLAYING){
            _UITimer += Time.deltaTime;
            _UI.SetTimer(Mathf.RoundToInt(_UITimer));
            GameManager.Instance.blackboard.timerPractice = Mathf.RoundToInt(_UITimer);
        }
    }

    public void AddKill(){
        _UI.AddPoint(0);
    }

    public AudioSource[] GetAllAudioSource(){
        return FindObjectsOfType<AudioSource>();
    }
}

