using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosingPlayers : MonoBehaviour
{
    List<PlayerData> playerList;
    public UnityEngine.UI.Text NumberOfPlayers;
    public GameObject ContinueText;
    public GameObject ControllersContainer;
    public Sprite KeyboardSprite;
    public Sprite ControllerSprite;
    public GameObject RoundChosing;

    void OnEnable()
    {
        playerList = new List<PlayerData>();
        for(int i = 0; i < 4; i++)
            ControllersContainer.transform.GetChild(i).gameObject.SetActive(false);
        //foreach(Transform tr in ControllersContainer.GetComponentsInChildren<Transform>()) if(tr != ControllersContainer.transform) tr.gameObject.SetActive(false);
        for(int i = 0; i < 4; i++)
            ContinueText.transform.GetChild(i).gameObject.SetActive(false);
        ContinueText.SetActive(false);
    }

    void Update()
    {
        foreach(PlayerData pd in GameManager.Instance.blackboard.playerData){
            if((pd.inputDevice.name == "Keyboard" && pd.InputFire) || (pd.InputJump && pd.inputDevice.name != "Keyboard")) {
                if(!playerList.Contains(pd)){
                    pd.SetId(playerList.Count);
                    playerList.Add(pd);
                }
                else{
                    if(pd.playerId == 0 && playerList.Count >= 2){
                        GameManager.Instance.blackboard.players = playerList.Count;
                        RoundChosing.SetActive(true);
                        gameObject.SetActive(false);
                    }
                }
            }
        }
        for(int i = 0; i < playerList.Count; i++)
        {
            Transform temp = default;

            if(i == 0){
                string controlScheme = playerList[i].playerInput.currentControlScheme;
                ContinueText.transform.Find(controlScheme).gameObject.SetActive(true);
                temp = ControllersContainer.transform.Find("Player1");
            }
            if(i == 1) temp = ControllersContainer.transform.Find("Player2");
            if(i == 2) temp = ControllersContainer.transform.Find("Player3");
            if(i == 3) temp = ControllersContainer.transform.Find("Player4");
            temp.gameObject.SetActive(true);
            temp.Find("Image").GetComponent<UnityEngine.UI.Image>().color = playerList[i].playerColor;
            temp.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite = (playerList[i].inputDevice.name == "Keyboard") ? KeyboardSprite : ControllerSprite;
            
        }
        NumberOfPlayers.text = playerList.Count + "/4 PLAYERS IN";
        if(playerList.Count >= 2) ContinueText.SetActive(true);
        else ContinueText.SetActive(false);
    }
}
