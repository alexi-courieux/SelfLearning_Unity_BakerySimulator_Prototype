using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [SerializeField] private AudioClipRefsDictionarySo audioClipRefsDictionarySo;
    [SerializeField] private Transform audioSource3dPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySound2D(AudioClipRefSo audioClipRef, float volumeMultiplier = 1f)
    {
        AudioClip clip = audioClipRef.audioClips[Random.Range(0, audioClipRef.audioClips.Length)];
        AudioSource.PlayClipAtPoint(clip, Vector3.zero, volumeMultiplier);
    }
    
    private Transform PlaySound3D(AudioClipRefSo audioClipRef, Vector3 position, bool loop = false,  float volumeMultiplier = 1f)
    {
        AudioClip clip = audioClipRef.audioClips[Random.Range(0, audioClipRef.audioClips.Length)];
        Transform source = Instantiate(audioSource3dPrefab, position, Quaternion.identity);
        AudioSource audioSource = source.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volumeMultiplier;
        audioSource.Play();
        Debug.Log(clip.length);
        return source;
    }
}
