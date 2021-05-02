using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShow : MonoBehaviour
{
    public WeaponShowSettings _wss;
    private Vector3 _startPos;
    private Facing _moving;
    private float _percent = 0.0f;

    void Start()
    {
        _startPos = transform.position;
        _moving = Facing.Up;
    }

    void Update()
    {
        if(_moving == Facing.Up && _percent < 1){
            _percent += Time.deltaTime * _wss._Speed;
            transform.position = Vector3.Lerp(_startPos + (Vector3.up * _wss._yOscillation), _startPos - (Vector3.up * _wss._yOscillation), _percent);
        }
        else if(_moving == Facing.Down && _percent < 1){
            _percent += Time.deltaTime * _wss._Speed;
            transform.position = Vector3.Lerp(_startPos - (Vector3.up * _wss._yOscillation), _startPos + (Vector3.up * _wss._yOscillation), _percent);
        }
        if(_percent >= 1){
            _percent = 0.0f;
            if(_moving == Facing.Up) _moving = Facing.Down;
            else _moving = Facing.Up;
        }

        transform.Rotate(Vector3.one, Time.deltaTime * _wss._RotationSpeed);
    }
}
