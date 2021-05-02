using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

struct PlayerUI{
    public GameObject g;
    public UnityEngine.UI.Image icon;
    public UnityEngine.UI.Text name;
    public UnityEngine.UI.Text score;
}

public class UIPlaying : MonoBehaviour
{
    public static UIPlaying ui;

    private GameObject _PlayerUIPrefab;
    public UnityEngine.UI.Text _RoundCounter;
    private PlayerUI[] _Players;
    public Blackboard b;
    public bool hasTimer = false;
    private bool GameIsPaused = false;
    private GameObject pauseMenuUI;

    void Awake(){
        ui = this;
        b = GameManager.Instance.blackboard;
        _PlayerUIPrefab = Resources.Load<GameObject>("Prefabs/UI/Player");

        int numberOfPlayers = b.players;

        _Players = new PlayerUI[numberOfPlayers]; 
        for(int i = b.players - 1; i >= 0; i--){
            _Players[i].g = Instantiate(_PlayerUIPrefab, transform);
            _Players[i].g.name = "Player" + (i + 1);
            _Players[i].name = _Players[i].g.transform.Find("Name").GetComponent<UnityEngine.UI.Text>();
            _Players[i].name.text = "Player " + (i + 1);
            _Players[i].icon = _Players[i].g.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();
            _Players[i].icon.color = (i == 0) ? ColorsExtension.Red : (i == 1) ? ColorsExtension.Blue : (i == 2) ? ColorsExtension.Green : ColorsExtension.Magenta;
            _Players[i].score = _Players[i].g.transform.Find("Score").GetComponent<UnityEngine.UI.Text>();
            float maxWidth = (256 * (numberOfPlayers - 1)) + 160;
            _Players[i].g.GetComponent<RectTransform>().anchoredPosition = new Vector2(-maxWidth + (i * 256), -160);
        }

        pauseMenuUI = transform.Find("Pause").gameObject;
        pauseMenuUI.transform.SetSiblingIndex(transform.childCount - 1);
    }

    void Start()
    {
        if(!hasTimer) _RoundCounter.text = b.RoundNumber.ToString();
        for(int i = 0; i < b.players; i++)
            _Players[i].score.text = b.GetScore(i).ToString();
    }  

    public void AddPoint(int idx){ b.SetScore(idx, true); }
    public void RemPoint(int idx){ b.SetScore(idx, false); }
    public void SetPoint(int idx, int val){ b.SetScore(idx, val); }

    public void SetTimer(int val) 
    {
        if(hasTimer) transform.Find("Timer").GetComponent<UnityEngine.UI.Text>().text = val.ToString();
    }
    

    void Update()
    {
        if(!hasTimer) _RoundCounter.text = "Round " + b.RoundNumber.ToString();
        for(int i = 0; i < b.players; i++)
            _Players[i].score.text = b.GetScore(i).ToString();

        if(GameManager.Instance.CheckState(StateType.PLAYING) || GameManager.Instance.CheckState(StateType.PRACTICE))
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
    }

    public void Resume()
    {
        Debug.Log("Resuming");
        Pause();
    }

    public void LoadMenu()
    {
        Debug.Log("Loading Menu...");
        if(GameManager.Instance.level)
            GameManager.Instance.level.State = LevelState.ENDGAME;
        else if(GameManager.Instance.pLevel)
            GameManager.Instance.pLevel.State = LevelState.ENDGAME;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    private void Pause(){
        if (GameIsPaused) GameIsPaused = false;
        else GameIsPaused = true;
        GameManager.Instance.Pause(GameIsPaused);
        pauseMenuUI.SetActive(GameIsPaused);
    }
}
