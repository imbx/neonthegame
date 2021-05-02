using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    public static AudioPeer main;
    public GameObject _PrefabBar;
    private AudioSource _audioSrc;
    private float[] _samples;
    [Range(32, 1024)]
    public int _numSamples;

    [Range(8, 32)]
    public int _numBands;
    private float[] _freqBand;
    private float[] _bandBuffer;
    private float[] _bufferDecrease;

    float[] _freqBandHighest;
    public static float[] _audioBand;
    public static float[] _audioBandBuffer;

    public static float _Amplitude, _AmplitudeBuffer;
    float _AmplitudeHighest;

    private GameObject[] _bars;
    private Material[] _barMaterials;
    [Range(1f, 128f)]
    public float _spacingBars = 1f;


    [Range(0.25f, 128f)]
    public float _startScale;
     [Range(1f, 512f)]
    public float _scaleMultiplier;

    public bool _usingBuffer = true;

    public Gradient _matGradient;
    public bool RotateBars = false;

    void Awake() {
        main = this;
        _samples = new float[_numSamples];
        if(_numBands > _numSamples * 0.5f) _numBands = (int)(_numSamples * 0.5f);
        _freqBand = new float[_numBands];
        _bandBuffer = new float[_numBands];
        _bufferDecrease = new float[_numBands];
        _freqBandHighest = new float[_numBands];
        _audioBand = new float[_numBands];
        _audioBandBuffer = new float[_numBands];
        _bars = new GameObject[_numBands];
        _barMaterials = new Material[_numBands];
        CreateObjects();
    }
    // Start is called before the first frame update
    void Start()
    {
        _audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        GetAmplitude();
        UpdateObjects();
    }

    private void GetSpectrumAudioSource(){
        _audioSrc.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void BandBuffer(){
        for(int g = 0; g < _numBands; g++){
            if(_freqBand[g] > _bandBuffer[g]){
                _bandBuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }
            if(_freqBand[g] < _bandBuffer[g]){
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;
            }
        }
    }

    private void MakeFrequencyBands(){
        int count = 0;
        float MathfPowMultiplier = _numSamples / Mathf.Pow(2, _numBands); 
        for(int i = 0; i < _numBands; i++){
            float average = 0;
            int sampleCount = (int)(Mathf.Pow(2, i) * MathfPowMultiplier);
            if(i == _numBands - 1) sampleCount += (int)MathfPowMultiplier;

            for(int j = 0; j < sampleCount; j++){
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;
            _freqBand[i] = average * 10;
        }
    }
    void CreateAudioBands(){
        for(int i = 0; i < _numBands; i++){
            if(_freqBand[i] > _freqBandHighest[i]){
                _freqBandHighest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void GetAmplitude(){
        float _CurrentAmplitude = 0;
        float _CurrentAmplitudeBuffer = 0;
        for (int i = 0; i < _numBands; i++){
            _CurrentAmplitude += _audioBand[i];
            _CurrentAmplitudeBuffer += _audioBandBuffer[i];
        }
        if(_CurrentAmplitude > _AmplitudeHighest){
            _AmplitudeHighest = _CurrentAmplitude;
        }
        _CurrentAmplitude = _CurrentAmplitude / _AmplitudeHighest;
        _AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;
    }


    private void CreateObjects(){
        Vector3 currentPosition = Vector3.zero;
        for(int i = 0; i < _numBands; i++){
            _bars[i] = Instantiate(_PrefabBar);
            _bars[i].transform.SetParent(transform);
            _bars[i].transform.localPosition = currentPosition;
            currentPosition += Vector3.right * _spacingBars;
            _barMaterials[i] = _bars[i].GetComponent<MeshRenderer>().material;
        }
        if(RotateBars) transform.eulerAngles = new Vector3(-15, 0, 0);
    }

    public void SetMatColor(Color main, Color second){
        for(int i = 0; i < _barMaterials.Length; i++){
            _barMaterials[i].SetColor("_FColor", main);
            _barMaterials[i].SetColor("_SColor", second);
        }
    }

    private void UpdateObjects(){
        if(_bars.Length > 0){
            for(int i = 0; i < _bars.Length; i++){
                Vector3 localScale = _bars[i].transform.localScale;
                Vector3 localPosition = _bars[i].transform.localPosition;
                float scaleMultiplierY = (_usingBuffer) ? (_bandBuffer[i] * _scaleMultiplier) : (_freqBand[i] * _scaleMultiplier);
                _bars[i].transform.localScale = new Vector3(localScale.x, scaleMultiplierY + _startScale, localScale.z);
                _bars[i].transform.localPosition = new Vector3(localPosition.x, ((scaleMultiplierY + _startScale) * 0.5f), localPosition.z);
                _barMaterials[i].SetFloat("_GradPosition", scaleMultiplierY / _scaleMultiplier);
                _bars[i].GetComponent<MeshRenderer>().material = _barMaterials[i];
            }
        }
    }
}
