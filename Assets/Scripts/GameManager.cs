using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;
public enum StateType
{
    DEFAULT,
    WAITING,
    PLAYING,
    PRACTICE,
    ENDPRACTICE,
    GAMEOVER,
    GAMESTART,
    LOBBY,
    MENU,
    OPTIONS
};

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private StateType currentState;
    public Blackboard blackboard;

    public LevelLoader level;
    public PracticeLoader pLevel;
    private GameObject _PrefabInputPlayer;

    void Awake(){
        if(instance != null){
            Debug.Log("Instance is already created wtf");
        }
        else{
            instance = this;
            DontDestroyOnLoad(instance);
            blackboard = new Blackboard();
            CurrentState(StateType.MENU);
            _PrefabInputPlayer = Resources.Load<GameObject>("Prefabs/PlayerInputs");
        }
    }   
 
    public static GameManager Instance {
        get { 
            return instance;
        }
    }

    public void CreateInput(InputDevice inputDevice){
        PlayerInput pi = PlayerInput.Instantiate(_PrefabInputPlayer, pairWithDevice: inputDevice);
        pi.transform.SetParent(transform);
        blackboard.playerData.Add(new PlayerData(inputDevice, pi));
        Debug.Log("Created playerinput with " + inputDevice.name + " with " + pi.currentControlScheme + " as scheme, now " + blackboard.playerData.Count);
        
        if(inputDevice.name == "Keyboard") {
            pi = PlayerInput.Instantiate(_PrefabInputPlayer, controlScheme: "Arrows", pairWithDevice: inputDevice);
            pi.transform.SetParent(transform);
            blackboard.playerData.Add(new PlayerData(inputDevice, pi));
            Debug.Log("Created playerinput with " + inputDevice.name + " with " + pi.currentControlScheme + " as scheme, now " + blackboard.playerData.Count);
        }
    }

    void Update(){
        if(currentState == StateType.PLAYING){
            if(blackboard.rounds == blackboard.Player1Score || blackboard.rounds == blackboard.Player2Score || blackboard.rounds == blackboard.Player3Score || blackboard.rounds == blackboard.Player4Score) {
                Debug.Log("Should end");
                CurrentState(StateType.GAMEOVER);
            }
        }
        //if(currentState == StateType.GAMEOVER){
            //if(Input.anyKey) CurrentState(StateType.MENU);
        //}
    }
 
    // Add your game mananger members here
    public void Pause(bool paused) {
         if(paused) {
            TimeScale = 0.0f;
        } else {
            TimeScale = 1.0f;
        }
    }
    public float TimeScale { 
        set{ 
            Time.timeScale = value; 
            if(level)
                foreach(AudioSource aS in level.GetAllAudioSource()){
                    aS.pitch = value;
                }
            if(pLevel){
                foreach(AudioSource aS in pLevel.GetAllAudioSource()){
                    aS.pitch = value;
                }
            }
        }
        get { return Time.timeScale;} 
    }

    public bool CheckState(StateType value){
        if(currentState == value) return true;
        return false;
    }

     public StateType CurrentState(StateType value = StateType.DEFAULT) {
        if(currentState != value && value != StateType.DEFAULT) {
            currentState = value;
 
            switch(value) {
                case StateType.MENU:
                    TimeScale = 1f;
                    SceneManager.LoadScene("MainMenu");
                    break;
                 case StateType.PRACTICE:
                    blackboard.ResetScores();
                    SceneManager.LoadScene("Practice");
                    break;
                case StateType.PLAYING:
                    blackboard.ResetScores();
                    SceneManager.LoadScene("Level1");
                    break;
                case StateType.GAMEOVER:
                    level.State = LevelState.ENDGAME;
                    break;
                default:
                    break;
            }
        }
        return currentState;
   }
}
