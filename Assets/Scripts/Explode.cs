using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public float cubeSize = 0.02f;
    public int cubesInRow = 7;
    float cubesPivotDistance;
    Vector3 cubesPivot;
    public float explosionForce = 50f;
    public float explosionRadius = 4f;
    public float explosionUpward = 0.4f;
    public bool _Explode = false;
    public GameObject cubePrefab;

    void Start()
    {
       cubesPivotDistance = cubeSize * cubesInRow / 2;
       cubesPivot = Vector3.one * cubesPivotDistance; 
    }

    void Update(){
        if(_Explode){
            _Explode = false;
            Expl();
        }
    }

    private void Expl(){
        List<Rigidbody> rbs = new List<Rigidbody>();
        gameObject.SetActive(false);
        for(int x = 0; x < cubesInRow; x++)
            for(int y = 0; y < cubesInRow; y++)
                    rbs.Add(CreatePiece(Random.Range(x - (cubeSize * 0.5f), x + (cubeSize * 0.5f)), Random.Range(y - (cubeSize * 0.5f), y + (cubeSize * 0.5f)), 0));
        Vector3 explosionPos = transform.position;
        foreach(Rigidbody rb in rbs){
                Vector2 mov = (explosionPos - rb.transform.position).normalized;
                rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, explosionUpward, ForceMode.Force);
        }
    }

    private Rigidbody CreatePiece(float x = 0, float y = 0, float z = 0){
        GameObject piece;

        float finalSize = Random.Range(cubeSize * 0.25f, cubeSize * 1.5f);

        piece = Instantiate(cubePrefab, transform.position + new Vector3(finalSize * x, finalSize * y, finalSize * z) - cubesPivot, Quaternion.identity);
        piece.transform.localScale = Vector3.one * finalSize;
        piece.GetComponent<Rigidbody>().mass = cubeSize;
        
        piece.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", transform.Find("Image").GetComponent<SpriteRenderer>().color * Mathf.Pow(2f, 2f));
        piece.GetComponent<MeshRenderer>().material.SetColor("_MainColor", transform.Find("Image").GetComponent<SpriteRenderer>().color);

        Destroy(piece, 3f);

        return piece.GetComponent<Rigidbody>();
    }
}
