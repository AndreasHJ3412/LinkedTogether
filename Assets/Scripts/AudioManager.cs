using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("------ Audio Source ------")]
    [SerializeField] private AudioSource backgroundSound;
    [SerializeField] private AudioSource soundEffects;


    [Header("------ Audio Clips ------")]
    public AudioClip forestSound;
    public AudioClip citySound;
    public AudioClip mountainSound;
    public AudioClip spaceSound;
    public AudioClip dogSound;
    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip landSound;

    [Header("------ Fade Settings ------")]
    public float fadeDuration = 0.5f;
    
    //bark
    private float barkTimer = 0f;
    private float nextBarkTime;

    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        backgroundSound.clip = forestSound;
        backgroundSound.loop = true;
        backgroundSound.Play();
        
        SetNextBarkTime();
    }
    
    private void Update()
    {
        HandleRandomBark();
    }

    
    public void PlaySoundEffects(AudioClip clip)
    {
        soundEffects.PlayOneShot(clip);
    }

    public void ChangeZone(ZoneTrigger.ZoneType zone)
    {
        AudioClip newClip = backgroundSound.clip;

        switch (zone)
        {
            case ZoneTrigger.ZoneType.City:
                newClip = citySound;
                break;
            case ZoneTrigger.ZoneType.Mountain:
                newClip = mountainSound;
                break;
            case ZoneTrigger.ZoneType.Space:
                newClip = spaceSound;
                break;
            case ZoneTrigger.ZoneType.Forest:
                newClip = forestSound;
                break;
        }

        if (backgroundSound.clip != newClip)
        {
            StartCoroutine(FadeToNewClip(newClip));
        }
    }

    private IEnumerator FadeToNewClip(AudioClip newClip)
    {
        float startVolume = backgroundSound.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            backgroundSound.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        backgroundSound.Stop();
        backgroundSound.clip = newClip;
        backgroundSound.loop = true;
        backgroundSound.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            backgroundSound.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        backgroundSound.volume = startVolume; // Ensure exact final volume
    }
    
    private void SetNextBarkTime()
    {
        nextBarkTime = Random.Range(7f, 15f);
    }
    
    private void HandleRandomBark()
    {
        barkTimer += Time.deltaTime;

        if (barkTimer >= nextBarkTime)
        {
            PlaySoundEffects(dogSound); // Bark!
            barkTimer = 0f;
            SetNextBarkTime();
        }
    }


}


