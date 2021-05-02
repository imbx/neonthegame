using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaylistController : MonoBehaviour
{
    public List<AudioClip> trackList;

    private int lastSong = -1;
    private AudioSource audioSource;
    private int currentSong = 0;

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        trackList.Shuffle();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!audioSource.isPlaying)
        {
            lastSong = currentSong;
            currentSong = Random.Range(0, trackList.Count);
            
            if(currentSong == lastSong) currentSong++;
            if (currentSong >= trackList.Count) currentSong = 0;

            audioSource.clip = trackList[currentSong];
            audioSource.Play();
        }
    }
}
