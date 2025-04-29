using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("------Audio Source------")]
    [SerializeField] private AudioSource backgroundSound; 
    [SerializeField] private AudioSource soundEffects;

    [Header("------Audio Clip------")]
    public AudioClip forestSound;
    public AudioClip citySound;
    public AudioClip mountainSound;
    public AudioClip spaceSound;
    public AudioClip dogSound;
    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backgroundSound.clip = forestSound;
        backgroundSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySoundEffects(AudioClip clip)
    {
        soundEffects.PlayOneShot(clip);
    }
}
