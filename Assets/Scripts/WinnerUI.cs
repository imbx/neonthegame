using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerUI : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isPractice = false;
    void Start()
    {
        string playerWinner = (isPractice) ? "PLAYER " + GameManager.Instance.blackboard.WinningPlayer + " WINS" : "YOU GOT " + GameManager.Instance.blackboard.Player1Score + " POINT/S\nIN " + GameManager.Instance.blackboard.timerPractice + " SECONDS";
        GetComponent<UnityEngine.UI.Text>().text = playerWinner;
    }
}