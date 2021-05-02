using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ProjectileSettings", menuName = "ScriptableObjects/ProjectileSettings", order = 1)]
public class ProjectileSettings : ScriptableObject
{
    public Gradient _MainGradient;
    [ColorUsage(true, true)]
    public Color _MainColor;
    [ColorUsage(true, true)]
    public Color _SecondColor;
    public float _BulletSpeed = 4f;
    [Range(0.05f, 5f)]
    public float _Lifetime = 2f;
    [Range(-1f, 5f)]
    public float _maxDistance = 1f;
    public float _CubeRotationSpeed = 240f;
    public Vector3 _CubeScale = Vector3.one * 0.05f;

    [Range(-1f, 5f)]
    public float _FollowTime = 1f;

    public float _colRad = 0.01f;
    public LayerMask _ground;
    public GameObject _TrailParticle;
}
