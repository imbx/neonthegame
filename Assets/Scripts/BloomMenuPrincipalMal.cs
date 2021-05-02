using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomMenuPrincipalMal : MonoBehaviour
{

    public GameObject SemiAlpha;
    private Vector3 scaleChange;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        scaleChange = new Vector3(-0.1f, -0.1f, -0.1f);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.localScale = scaleChange + new Vector3(10f + Mathf.Sin(Time.time), 50.0f, 0.0f);
        
    }

   
}
