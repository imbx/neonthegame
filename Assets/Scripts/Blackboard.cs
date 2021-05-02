using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;

public class Blackboard
{
    public int players = 0;
    public int rounds = 0;

    public int Player1Score = 0;
    public int Player2Score = 0;
    public int Player3Score = 0;
    public int Player4Score = 0;
    public Transform BulletContainer;
    public int timerPractice = 0;
    public List<PlayerData> playerData = new List<PlayerData>();

    public bool isDeviceAssigned(InputDevice id){
        if(playerData.Count > 0){
            foreach(PlayerData pd in playerData){
                if(pd.IsSameDevice(id)) return true;
            }
        }
        return false;
    }

    public PlayerData GetPlayerDataWithId(int idx){ 
        if(playerData.Count > 0){
            foreach(PlayerData pd in playerData){
                if(pd.playerId == idx) return pd;
            }
        }
        return null;
    }

    public int RoundNumber{
        get{
            return Player1Score + Player2Score + Player3Score + Player4Score + 1;
        }
    }

    public int WinningPlayer{
        get{
            int maxval = -1;
            int plId = -1;
            for(int i = 0; i < players; i++){
                if(maxval < GetScore(i)){
                    maxval = GetScore(i);
                    plId = i;
                }else if(maxval == GetScore(i)){
                    plId = -1;
                }
            }
            return plId;
        }
    }

    public int GetScore(int idx) {
        if(idx == 0) return Player1Score;
        else if(idx == 1) return Player2Score;
        else if(idx == 2) return Player3Score;
        else return Player4Score;
    }
    public void SetScore(int idx, bool add = true) {
        switch(idx){
            case 0: 
                if(add) Player1Score++;
                else Player1Score--;
                break;
            case 1:
                if(add) Player2Score++;
                else Player2Score--;
                break;
            case 2:
                if(add) Player3Score++;
                else Player3Score--;
                break;
            case 3:
                if(add) Player4Score++;
                else Player4Score--;
                break;

        }
    }

    public void SetScore(int idx, int val) {
        switch(idx){
            case 0: 
                Player1Score = val;
                break;
            case 1:
                Player2Score = val;
                break;
            case 2:
                Player3Score = val;
                break;
            case 3:
                Player4Score = val;
                break;

        }
    }

    public void ResetScores(){
        Player1Score = Player2Score = Player3Score = Player4Score = 0;
    }
    
}
