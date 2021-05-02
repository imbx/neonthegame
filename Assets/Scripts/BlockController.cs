using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BlockType{
    DEFAULT,
    EXIT,
    RESET
}

public class BlockController : MonoBehaviour
{
    [SerializeField]
    private Vector3 Position = Vector3.zero;
    [SerializeField]
    private Vector2 Size = Vector2.one;
    public float smoothFactor = 2;
    public float LowestYPos = -5f;
    [SerializeField]
    private float Margin = 0.05f;
    [SerializeField]
    private bool isOut = true;
    [SerializeField]
    private bool hasToMove = false;
    public float MaxScale = 0.75f;
    public bool HasToMove { get { return hasToMove; } } // From level loader, check last block if its placed with this
    public bool IsOut { get { return isOut; } }
    public float Speed = 2f;
    public bool movementBlock = false;
    public Vector2 maxMovementSize = Vector2.zero;
    public float movementBlockSpeed = 0.25f;
    private Vector2 transformedMovement = Vector2.zero;
    private bool movingAway = true;
    public LayerMask groundLayer;

    public void Move(){ 
        hasToMove = true; 
        if(!isOut) Position += Vector3.up * LowestYPos;
    }

    void Awake(){
        Position = transform.position;
        transform.position += Vector3.up * LowestYPos;
        isOut = true;

        if(movementBlock){
            GameObject col = new GameObject("Collider", typeof(BoxCollider2D));
            col.transform.SetParent(transform);

            col.layer = LayerMask.NameToLayer("Ground");;
            col.transform.localPosition = Vector3.zero;

            BoxCollider2D bc2d = col.GetComponent<BoxCollider2D>();
            bc2d.size = Size;
        }
    }   

    void Update()
    {
        if(hasToMove){
            if(isOut && transform.position.y == LowestYPos) transform.position = new Vector3(Position.x, LowestYPos, Position.z);

            transform.position = Vector3.Lerp(transform.position, (isOut) ? Position : new Vector3(transform.position.x, LowestYPos, transform.position.z), Time.deltaTime * smoothFactor * Speed);
            
            if(isOut && transform.position.y >= Position.y - Margin) {
                transform.position = Position;
                hasToMove = false;
                isOut = false;
            }
            else if(!isOut && transform.position.y <= Position.y + Margin){
                transform.position = Position;
                hasToMove = false;
                isOut = true;
            }
        }
        else if(!hasToMove && !isOut && movementBlock) {
            if(movingAway) transformedMovement += maxMovementSize * Time.deltaTime * movementBlockSpeed;
            else transformedMovement -= maxMovementSize * Time.deltaTime * movementBlockSpeed;
            
            if(transformedMovement.y - 0.05f < 0 && !movingAway) movingAway = true;
            
            if(transformedMovement.y + 0.05f > maxMovementSize.y && movingAway) movingAway = false;

            transform.position = Position + (Vector3)transformedMovement;
        }
    }

    void OnDrawGizmos(){
        #if UNITY_EDITOR
        if(movementBlock){
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                transform.position,
                transform.position + (Vector3)maxMovementSize
            );
            Gizmos.color = Color.white;
        }
        #endif
    }
}
