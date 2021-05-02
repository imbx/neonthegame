using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    private Vector3 direction = Vector3.zero;
    private float speed = 0;
    private float destroyTime = 1f;
    private float currentTime = 0f;

    void Update()
    {
        currentTime += Time.deltaTime;
        Color col = transform.Find("Sphere").GetComponent<SpriteRenderer>().material.GetColor("_BaseColor");
        col = new Color(col.r, col.g, col.b, 0.9f - (currentTime/destroyTime));
        transform.Find("Sphere").GetComponent<SpriteRenderer>().material.SetColor("_BaseColor", col);
        if(currentTime >= destroyTime) Destroy(gameObject);
    }

    void FixedUpdate()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void SetMovement(Vector3 dir, float spd, float destr){
        direction = -dir;
        speed = spd;
        destroyTime = destr * 0.75f;
    }
}
