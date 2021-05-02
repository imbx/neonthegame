using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;

public class MainMenu : MonoBehaviour
{
    void Update(){
        foreach(InputDevice id in InputSystem.devices){
            if(!GameManager.Instance.blackboard.isDeviceAssigned(id)){
                GameManager.Instance.CreateInput(id);
            }
        }
    }
    public void PracticeMode(){
        GameManager.Instance.blackboard.players = 1;
        GameManager.Instance.CurrentState(StateType.PRACTICE);
    }
    public void PlayersPressed(int players){
        GameManager.Instance.blackboard.players = players;
        
    }
    public void RoundsChoosen(int rounds)
    {
        GameManager.Instance.blackboard.rounds = rounds;
        GameManager.Instance.CurrentState(StateType.PLAYING);
    }
    public void QuitPressed()
    {
        Application.Quit();
    }
}
