using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings", order = 1)]
public class PlayerSettings : ScriptableObject
{
    [Range(0.01f, 25f)]
    public float _moveSpeed = 3f;
    [Range(0f, 0.99f)]
    public float _easeMultiplier = 0.75f;
    [Range(0f, 0.99f)]
    public float _shiftedMultiplier = 0.75f;
    [Range(0.01f, 1f)]
    public float _distanceMultiplier = 0.5f;
    [Range(0.01f, 0.99f)]
    public float _feetRadius = 0.01f;
    [Range(0.01f, 25f)]
    public float _jumpForce = 3f;
    [Range(0.01f, 1f)]
    public float _jumpTime = 0.15f;
    public LayerMask _ground;
    public float _bloomBase = 1.01f;
    public float _minTrailSize = 0.15f;
    public float _maxHeightExtension = 0.28f;
    public float _jumpAnimSpeed = 50f;
}
