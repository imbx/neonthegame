using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Controls;

public class PlayerData
{
    public int playerId = -1;
    public PlayerInput playerInput;
    public InputsHandler inputsHandler;
    public InputDevice inputDevice;
    public Color playerColor = ColorsExtension.Red;

    public PlayerData(InputDevice id, PlayerInput pi){
        inputDevice = id;
        playerInput = pi;
        inputsHandler = pi.GetComponent<InputsHandler>();
    }

    public void SetId(int idx){
        playerId = idx;

        switch(idx){
            case 0:
                playerColor = ColorsExtension.Red;
                break;
            case 1:
                playerColor = ColorsExtension.Blue;
                break;
            case 2:
                playerColor = ColorsExtension.Green;
                break;
            case 3:
                playerColor = ColorsExtension.Magenta;
                break;
            default:
                break;
        }
    }

    public bool IsSameDevice(InputDevice id) {
        return id == inputDevice;
    }

    public float InputHoriz{
        get { return inputsHandler.InputHoriz;}
    }
    public bool InputJump{
        get { return inputsHandler.InputJump; }
    }
    public bool InputShift{
        get { return inputsHandler.InputShift; }
    }
    public bool InputFire{
        get { return inputsHandler.InputFire; }
    }
}
