using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
     [Range(0.15f, 20f)]
    public float _Width = 2f;

    public Vector3 GetPosition()
    {
        return transform.position  - (Vector3.right * _Width * 0.5f) + (Vector3.right * Random.Range(0f, _Width));
    }

    void OnDrawGizmos(){
        #if UNITY_EDITOR
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                transform.position - (Vector3.right * _Width * 0.5f),
                transform.position + (Vector3.right * _Width * 0.5f)
            );
            Gizmos.color = Color.white;
        #endif
    }
}
