using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputsHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerController movement;
    [SerializeField]
    private float i_horizontal = 0;
    private bool b_jump = false;
    private bool b_shift = false;
    private bool b_fire = false;

    public float InputHoriz{
        get { return i_horizontal;}
    }

    public bool InputJump{
        get { return b_jump; }
    }
    public bool InputShift{
        get { return b_shift; }
    }

    public bool InputFire{
        get { return b_fire; }
    }

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        var players = FindObjectsOfType<PlayerController>();
        var index = playerInput.playerIndex;
        foreach(PlayerController bm in FindObjectsOfType<PlayerController>()){
            if(bm.Idx == index) movement = bm;
        }
    }

    public void OnMoveInput(CallbackContext context)
    {
        i_horizontal = context.ReadValue<float>();
    }

    public void OnVerticalInput(CallbackContext context){
        if(context.ReadValue<float>() == 1f) {
            b_jump = true;
            b_shift = false;
        }
        else if(context.ReadValue<float>() == -1f) {
            b_shift = true;
            b_jump = false;
        }
        else{
            b_jump = false;
            b_shift = false;
        }
    }

    public void OnFireInput(CallbackContext context){
        b_fire = context.performed;
    }
}
