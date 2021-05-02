using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelLoader : MonoBehaviour
{
    private GameObject _PrefabBackground;
    private GameObject[] _PrefabTerrains;
    private GameObject _PrefabPlayer;
    private GameObject _PrefabUIPlaying;
    private GameObject _PrefabInputPlayer;
    private GameObject _PrefabUIWinner;

    private GameObject _Background;
    private GameObject _UIPlaying;
    private GameObject _UIWinner;
    private GameObject _Terrain;

    private PlayerSpawner[] _PlayerSpawners;
    private List<GameObject> _Players;
    private List<BlockController> Blocks;
    private GameObject _DeathZone;
    private GameObject _Bullets;
    private int _LastLevelId = 0;
    public Transform BulletContainer{
        get{ return _Bullets.transform; }
    }

    private UIPlaying _UI;
    private bool Reset = false;
    private PlayerInputManager _playerInputManager;
    private List<InputsHandler> _Inputs;
    private LevelData CurrentData = default;
    [HideInInspector]
    public LevelState State = LevelState.FIRSTLOAD;
    public float secondsBetweenBlock = 0.1f;
    private Material _MatBlock;
    private Material _MatBeat;

    public float SlowmoMinScale = 0.25f;
    public float SlowmoMultiplier = 1f;

    void Awake()
    {
        GameManager.Instance.level = this;
        _PrefabBackground = Resources.Load<GameObject>("Prefabs/Level/Background");
        _PrefabTerrains = Resources.LoadAll<GameObject>("Prefabs/Level/Terrains/");
        _PrefabPlayer = Resources.Load<GameObject>("Prefabs/PlayerPrefab");
        _PrefabInputPlayer = Resources.Load<GameObject>("Prefabs/PlayerInputs");
        _PrefabUIPlaying = Resources.Load<GameObject>("Prefabs/UI/UIPlaying");
        _PrefabUIWinner = Resources.Load<GameObject>("Prefabs/UI/UIWinner");

        _MatBlock = Resources.Load<Material>("Materials/BlockLevel/BlockMat");
        _MatBeat = Resources.Load<Material>("Materials/BlockLevel/BeatMat");

        _Background = Instantiate(_PrefabBackground);
        _UIPlaying = Instantiate(_PrefabUIPlaying);
        _UIWinner = Instantiate(_PrefabUIWinner);

        _UI = _UIPlaying.GetComponent<UIPlaying>();
        _Inputs = new List<InputsHandler>();
        State = LevelState.FIRSTLOAD;

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
        while(GameManager.Instance.CurrentState() == StateType.PLAYING || GameManager.Instance.CurrentState() == StateType.GAMEOVER){
            switch(State){
                case LevelState.FIRSTLOAD:
                {
                    for(int i = 0; i < GameManager.Instance.blackboard.players; i++){
                        _Inputs.Add(GameManager.Instance.blackboard.GetPlayerDataWithId(i).inputsHandler);
                    }
                    State = LevelState.SEEDING;
                    break;
                }
                case LevelState.SEEDING:
                {
                    int levelId = (int)Random.Range(0, _PrefabTerrains.Length);
                    if (levelId != _LastLevelId){
                        _LastLevelId = levelId;
                        State = LevelState.LOADINGDATA;
                    }
                    break;
                }
                
                case LevelState.LOADINGDATA:
                {
                    Blocks = new List<BlockController>(); 
                    _Terrain = Instantiate(_PrefabTerrains[_LastLevelId]);
                    
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
                    _PlayerSpawners = _Terrain.GetComponentsInChildren<PlayerSpawner>();

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

                    AudioPeer.main.SetMatColor(CurrentData._BarsMainColor, CurrentData._BarsSecondColor);

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

                    int startPlayerSpawner = Random.Range(0, _PlayerSpawners.Length - 1);

                    GameObject go;
                    for(int i = 0; i < GameManager.Instance.blackboard.players; i++){
                        go = Instantiate(_PrefabPlayer);

                        go.transform.position = _PlayerSpawners[startPlayerSpawner].GetPosition();
                        startPlayerSpawner++;
                        if(startPlayerSpawner >= _PlayerSpawners.Length) startPlayerSpawner = 0;
                        PlayerController pc = go.GetComponent<PlayerController>();
                        pc.Create(i, _Inputs[i]);
                        _Players.Add(go);

                        yield return null;
                    }

                    State = LevelState.PLAYING;
                    break;
                }
                case LevelState.PLAYING:
                {
                    _MatBeat.SetColor("_EmissionColor", CurrentData._ScenaryColor * Mathf.Pow(2f, AudioPeer._AmplitudeBuffer));
                    
                    int dead = 0;
                    foreach(GameObject pl in _Players){
                        PlayerController pc = pl.GetComponent<PlayerController>();
                        if(pc.IsDead){
                            dead++;
                        }

                        if(Reset && !pc.IsDead){
                            _UI.AddPoint(pc.Idx);
                            break;
                        }
                        yield return null;
                    }

                    if(Reset){
                        Reset = false;
                        dead = 0;
                        State = LevelState.REMOVINGLEVEL;                        
                    }

                    if(_Players.Count - 1 <= dead) Reset = true;
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

                    Debug.Log("Level Out");
                    
                    State = LevelState.RESETINGLEVEL;
                    break;
                }
                case LevelState.RESETINGLEVEL:
                {
                    Debug.Log("Destroying Level");
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

                    Material winnerBg = _UIWinner.transform.Find("Background").GetComponent<UnityEngine.UI.Image>().material;
                    float blurVal = 0;
                    winnerBg.SetFloat("_BlurValue", blurVal);
                    _UIWinner.transform.Find("Background").gameObject.SetActive(true);

                    while(winnerBg.GetFloat("_BlurValue") < 0.002f){
                        blurVal += Time.deltaTime * 0.025f;
                        winnerBg.SetFloat("_BlurValue", blurVal);
                        yield return null; 
                    }

                    winnerBg.SetFloat("_BlurValue", 0.002f);
                    int winningPl = GameManager.Instance.blackboard.WinningPlayer;
                    string playerWinner = "PLAYER " + (winningPl + 1) + " WINS";
                    if(GameManager.Instance.blackboard.RoundNumber <= 1 || winningPl < 0){
                        playerWinner = "NO ONE WINS";
                    }
                    
                    _UIWinner.transform.Find("Winner").GetComponent<UnityEngine.UI.Text>().text = playerWinner;
                    _UIWinner.transform.Find("Winner").gameObject.SetActive(true);

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

    public AudioSource[] GetAllAudioSource(){
        return FindObjectsOfType<AudioSource>();
    }
}

