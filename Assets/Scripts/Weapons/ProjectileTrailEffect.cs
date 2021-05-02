using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrailEffect : MonoBehaviour
{
    private ProjectileSettings pSettings;
    private Vector3 _Direction = Vector3.zero;
    private float timeBetweenSpawns = 0f;
    
    void Update()
    {
        float rand = Random.Range(0.0f, 1.00f);
        if(rand < 0.5f){
            if(rand < 0.25f) timeBetweenSpawns = 0f;
            if(timeBetweenSpawns <= 0){
                Vector3 randSpawn = new Vector3(Random.Range(-0.025f, 0f), Random.Range(-0.025f, 0.025f), Random.Range(-0.05f, 0.05f));
                GameObject instance = (GameObject)Instantiate(pSettings._TrailParticle, transform.position + randSpawn, Quaternion.identity);
                instance.transform.SetParent(transform);
                instance.GetComponent<BulletParticle>().SetMovement(_Direction, pSettings._BulletSpeed, pSettings._FollowTime - 0.25f);
                instance.transform.Find("Sphere").GetComponent<SpriteRenderer>().material.SetColor("_BaseColor", pSettings._MainColor);
                instance.transform.Find("Sphere").GetComponent<SpriteRenderer>().material.SetColor("_EmissionColor", pSettings._MainColor);
                instance.transform.Find("Trail").GetComponent<TrailRenderer>().colorGradient = pSettings._MainGradient;
                timeBetweenSpawns = pSettings._FollowTime + 0.5f;
            }
        }
        timeBetweenSpawns -= Time.deltaTime;
    }

    public void SetVariables(Vector3 direction, ProjectileSettings ps){
        pSettings = ps;
        _Direction = direction;
    }
}
