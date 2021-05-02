using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    private GameObject _PracticePrefab;

    [Range(0.15f, 20f)]
    public float _Width = 2f;

    [Range(0.01f, 20f)]
    public float _NextSpawnTimer = 2f;
    private float _CurrentTimer = -2f;
    public bool HasGround = false;
    public GameObject childSpawned;
    public bool spawnOnGround = false;
    private Transform HitTransform;

    void Awake()
    {
        _PracticePrefab = Resources.Load<GameObject>("Prefabs/PracticeBot");
    }

    private void FixedUpdate() {
        if(spawnOnGround){
            RaycastHit2D hit = Physics2D.Raycast(transform.position, - Vector2.up);
            if(hit.collider != null){
                if(hit.transform.name == "Collider") {
                    HasGround = true;
                    HitTransform = hit.transform;
                }
                else HasGround = false;
            }
        }
    }


    void Update()
    {
        if(!childSpawned){
            if(spawnOnGround){
                if(HasGround && _CurrentTimer >= _NextSpawnTimer){
                    _CurrentTimer = 0;
                    Vector3 pos = transform.position  - (Vector3.right * _Width * 0.5f) + (Vector3.right * Random.Range(0f, _Width));
                    childSpawned = Instantiate(_PracticePrefab, pos, Quaternion.identity, HitTransform);
                    childSpawned.GetComponent<PracticeBot>().Create(GetComponent<Weapon>());
                }
            }
            else if(_CurrentTimer >= _NextSpawnTimer){
                _CurrentTimer = 0;
                Vector3 pos = transform.position  - (Vector3.right * _Width * 0.5f) + (Vector3.right * Random.Range(0f, _Width));
                childSpawned = Instantiate(_PracticePrefab, pos, Quaternion.identity, transform);
                childSpawned.GetComponent<PracticeBot>().Create(GetComponent<Weapon>());
            }
            _CurrentTimer += Time.deltaTime;
        }   
    }

    void OnDrawGizmos(){
        #if UNITY_EDITOR
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(
                transform.position - (Vector3.right * _Width * 0.5f),
                transform.position + (Vector3.right * _Width * 0.5f)
            );
            Gizmos.color = Color.white;
        #endif
    }
}
