using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShow", menuName = "ScriptableObjects/WeaponShow", order = 1)]
public class WeaponShowSettings : ScriptableObject
{
    public float _RotationSpeed = 50f;
    [Range(0.001f, 5f)]
    public float _yOscillation = 0.1f;
    public float _Speed = 1f;
}