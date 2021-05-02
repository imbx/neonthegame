using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileSettings pSettings;
    private Vector3 _Dir = Vector3.zero;
    [Range(0.01f, 5f)]
    private float _TimeCounter = 0f;
    private Vector3 _DistanceCounter = Vector3.zero;
    private Transform _ChildCube;
    private Transform _Trail;
    private Transform _Collider;
    private ProjectileTrailEffect _Pte;

    public virtual void Start()
    {
        _ChildCube = transform.Find("Cube");
        _Trail = transform.Find("Trail");

        _Collider = new GameObject("Point").transform;
        _Collider.SetParent(transform);
        _Collider.localPosition = Vector3.zero;

        _ChildCube.localScale = pSettings._CubeScale;
    }

    public virtual void OnEnable(){
        _ChildCube = transform.Find("Cube");
        _Trail = transform.Find("Trail");
        _DistanceCounter = transform.position;
        _Pte = GetComponent<ProjectileTrailEffect>();

        if(_Pte) _Pte.SetVariables(_Dir, pSettings);
        _Trail.GetComponent<TrailRenderer>().time = pSettings._FollowTime;
        _Trail.GetComponent<TrailRenderer>().colorGradient = pSettings._MainGradient;

        _ChildCube.GetComponent<MeshRenderer>().material.SetColor("_SColor", pSettings._MainColor);
        _ChildCube.GetComponent<MeshRenderer>().material.SetColor("_FColor", pSettings._SecondColor);

        _TimeCounter = 0;
    }

    public virtual void FixedUpdate()
    {
        
    }

    public virtual void Update()
    {
        _ChildCube.Rotate(Vector3.one * Time.deltaTime * pSettings._CubeRotationSpeed);
        _TimeCounter += Time.deltaTime;

        if(pSettings._maxDistance > 0){
            if(Vector3.Distance(_DistanceCounter, transform.position) >= pSettings._maxDistance) Destroy(gameObject);
        }

        if(_TimeCounter >= pSettings._Lifetime) Destroy(gameObject);

        if(Physics2D.OverlapCircle(_Collider.position, pSettings._colRad, pSettings._ground)) 
            Destroy(gameObject);

        transform.Translate(_Dir * pSettings._BulletSpeed * Time.deltaTime);
    }

    public void SetData(ProjectileSettings ps, Vector3 dir){
        pSettings = ps;
        _Dir = dir;
        transform.gameObject.SetActive(true);
    }

    private void UpdateProjectile(){
        _TimeCounter = 0;
        GetComponent<TrailRenderer>().colorGradient = pSettings._MainGradient;
        GetComponent<SpriteRenderer>().color = pSettings._MainColor;
    }
}
