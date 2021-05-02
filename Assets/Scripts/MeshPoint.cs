using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPoint : MonoBehaviour
{
    private int _idHorizontal;
    private int _idVertical;
    private LineRenderer _lineVertical;
    private LineRenderer _lineHorizontal;

    private float _yStaticPoint = 0;
    [Range(0.01f, 5f)]
    public float _yOscillation = 0.15f;

    private Vector3 _yDestination = Vector3.zero;

    private float _ySpeed = 3f;

    private bool _blockAnimation = false;
    
    // Start is called before the first frame update
    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        if(!_blockAnimation){
            UpdateLineRenderer();
            MoveLines();
        }
    }
    private void UpdateLineRenderer(){
        if(_lineVertical) _lineVertical.SetPosition(_idVertical, transform.position);
        if(_lineHorizontal) _lineHorizontal.SetPosition(_idHorizontal, transform.position);
    }
    private void MoveLines(){
        if(transform.position == _yDestination){
            _ySpeed = Random.Range(1f, 10f);
            float newYValue = Random.Range(-_yOscillation,_yOscillation);
            _yDestination = new Vector3(transform.position.x, _yStaticPoint + newYValue, transform.position.z);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _yDestination, _ySpeed * Time.deltaTime);
        }
    }

    public void SetIds(int x, int z){
        _idHorizontal = x;
        _idVertical = z;
        _yDestination = transform.position;
        _ySpeed = Random.Range(1f, 10f);
    }

    public void SetStaticY(float yPos){
        _yStaticPoint = yPos;
    }

    public void SetVertical(LineRenderer lr){
        _lineVertical = lr;
    }
    public void SetHorizontal(LineRenderer lr){
        _lineHorizontal = lr;
    }

    public void BlockAnimation(bool isIt){
        _blockAnimation = isIt;
        UpdateLineRenderer();
        MoveLines();
    }
}
