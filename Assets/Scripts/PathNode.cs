using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int gridXPos;
    public int gridYPos;
    
    public bool isWall;
    public Vector3 Position;
    public PathNode Parent;

    public int gCost;
    public int hCost;
    public int FCost { get { return gCost + hCost; } }

    public PathNode(bool _isWall, Vector3 _position, int _gridX, int _gridY){
        isWall = _isWall;
        Position = _position;
        gridXPos = _gridX;
        gridYPos = _gridY;
    }
}
