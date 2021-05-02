using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    private Mesh mesh;
    public Material material;
    public Material backgroundMaterial;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 7;
    private float F_height = 8f;
    private List<Vector2> LV_maxHeight;

    private Vector3 _startPoint = Vector3.zero;
    private float _sizeMultiplier = 0.75f;

    public float _wireWidth = 0.01f;

    public float _backgroundDepth = 20f;
    public Gradient _lineGradient;

    private List<LineRenderer> _lineHorizontal;
    private List<LineRenderer> _lineVertical;
    
    void Start()
    {
        mesh = new Mesh();
        _lineHorizontal = new List<LineRenderer>();
        _lineVertical = new List<LineRenderer>();
        
        if(!GetComponent<MeshFilter>()) gameObject.AddComponent<MeshFilter>();
        GetComponent<MeshFilter>().mesh = mesh;

        LV_maxHeight = new List<Vector2>();
        LV_maxHeight.Add(new Vector2(0, 0));
        LV_maxHeight.Add(new Vector2(5f, 1f));
        LV_maxHeight.Add(new Vector2(8f, 2f));
        LV_maxHeight.Add(new Vector2(12f, 5f));
        LV_maxHeight.Add(new Vector2(14f, 6f));
        LV_maxHeight.Add(new Vector2(zSize - 1f, 7f));

        _sizeMultiplier = transform.localScale.x;
        _startPoint = transform.position;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape(){
        if(F_height < 6f) F_height = 6f;
        vertices = new Vector3[(xSize + 1) * 3];

        float[] highestY = new float[(xSize + 1)];
        //LineRenderer[] lr = new LineRenderer[xSize + 1];

        for(int i = 0; i <= xSize; i++){
            GameObject test = new GameObject("Vertical Line " + i, typeof(LineRenderer));
            test.transform.SetParent(transform);
            
            LineRenderer lr = test.GetComponent<LineRenderer>();

            lr.positionCount = zSize + 1;
            lr.colorGradient = _lineGradient;
            lr.material = material;
            lr.startWidth = _wireWidth;
            lr.endWidth = _wireWidth;

            _lineVertical.Add(lr);
            
        }
        GameObject pointsContainer = new GameObject("Points container");
        pointsContainer.transform.position = Vector3.zero;
        pointsContainer.transform.SetParent(transform);
        

        for(int i = 0, h = 0, z = 0; z <= zSize; z++){
            GameObject lineHorizontal = new GameObject("Horizontal Line " + z, typeof(LineRenderer));
            lineHorizontal.transform.SetParent(transform);
            LineRenderer horizLr = lineHorizontal.GetComponent<LineRenderer>();
            horizLr.positionCount = xSize + 1;
            horizLr.material = material;
            horizLr.startColor = _lineGradient.Evaluate((1f * z)/zSize);
            horizLr.endColor = _lineGradient.Evaluate((1f * z)/zSize);
            horizLr.startWidth = _wireWidth;
            horizLr.endWidth = _wireWidth;

            _lineHorizontal.Add(horizLr);
            
            for(int x = 0; x <= xSize; x++){

                if(LV_maxHeight[h].x == z)
                {
                    F_height = LV_maxHeight[h].y;
                    if(LV_maxHeight.Count - 1 > h) h++;
                } 

                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * F_height;
                float currentY = _startPoint.y + (y * _sizeMultiplier);

                if(currentY > highestY[x]){
                    highestY[x] = currentY;
                }

                if(z == zSize){
                    if(currentY < highestY[x] + 0.15f) currentY = highestY[x] + 0.25f;
                }

                GameObject point = new GameObject("Vertex " + x + " " + z, typeof(MeshPoint));
                point.transform.SetParent(pointsContainer.transform);
                point.transform.position = new Vector3(_startPoint.x + (x * _sizeMultiplier),currentY,_startPoint.z + (z * _sizeMultiplier));

                point.GetComponent<MeshPoint>().SetIds(x, z);
                point.GetComponent<MeshPoint>().SetHorizontal(horizLr);
                point.GetComponent<MeshPoint>().SetVertical(_lineVertical[x]);
                point.GetComponent<MeshPoint>().SetStaticY(_startPoint.y + (y * _sizeMultiplier));
                
                if(z == zSize) {
                    point.GetComponent<MeshPoint>().BlockAnimation(true);
                    vertices[x] = new Vector3(x, (currentY - _startPoint.y) / _sizeMultiplier, z);
                    vertices[x + xSize] = new Vector3(x, -_backgroundDepth, z);
                }

                i++;
            }
        }

        triangles = new int[xSize * 2 * 6];

        int vert = 0;
        int tris = 0;

        for(int z = 0; z < 2; z++){
            for(int x = 0; x < xSize; x++){
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                
                vert++;
                tris += 6;
            }
            vert++;
        }
        //UpdateGradient();
    }

    void UpdateMesh(){
        mesh.Clear();
        mesh.vertices = vertices;
        Array.Reverse(triangles);
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        //mesh.Optimize();
        GetComponent<MeshRenderer>().material = backgroundMaterial;
    }

    public void SetGradient(Gradient gr){
        _lineGradient = gr;
    }

    private void UpdateGradient(){
        foreach(LineRenderer lr in _lineVertical) lr.colorGradient = _lineGradient;
        for(int i = 0; i < _lineHorizontal.Count; i++) {
            _lineHorizontal[i].startColor = _lineGradient.Evaluate((1f * i)/zSize);
            _lineHorizontal[i].endColor = _lineGradient.Evaluate((1f * i)/zSize);
        }
    }

    void Update(){
        if(_lineVertical.Count > 0){
            if(_lineVertical[0].colorGradient != _lineGradient) UpdateGradient();
        }
    }
}
