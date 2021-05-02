using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{   
    [Range(0.15f, 20f)]
    public float _Width = 2f;
    [Range(0.01f, 1f)]
    public float _SpawnRate = 0.5f;
    [Range(0.01f, 20f)]
    public float _NextSpawnTimer = 5f;
    private float _CurrentTimer = 0;
    public int _MaxSpawnItems;
    private List<GameObject> _PrefabObjects;
    
    void Awake(){
        _PrefabObjects = new List<GameObject>();
        GameObject[] TemporalGO = Resources.LoadAll<GameObject>("Prefabs/Objects/");
        foreach(GameObject go in TemporalGO) _PrefabObjects.Add(go);
    }

    void Update()
    {
        if(transform.childCount < _MaxSpawnItems){
            if(_CurrentTimer >= _NextSpawnTimer){
                if(Random.Range(0.01f, 1.00f) <= _SpawnRate){
                    _CurrentTimer = 0;
                    Vector3 pos = transform.position  - (Vector3.right * _Width * 0.5f) + (Vector3.right * Random.Range(0f, _Width));
                    Instantiate(_PrefabObjects[Random.Range(0, _PrefabObjects.Count)], pos, Quaternion.identity, transform);
                }
            }
            else _CurrentTimer += Time.deltaTime;
        }   
    }

    void OnDrawGizmos(){
        #if UNITY_EDITOR
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                transform.position - (Vector3.right * _Width * 0.5f),
                transform.position + (Vector3.right * _Width * 0.5f)
            );
            Gizmos.color = Color.white;
        #endif
    }
}
