using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPlatform : MonoBehaviour
{
    public bool GodSpeaking;
    public GameObject GodSpeaking_Canvas;

    public List<GameObject> StorySlides;
    public int StorySlideNumber;


    public void Update()
    {
        if (GodSpeaking == true)
        {
            StartCoroutine(Speaking());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GodSpeaking = true;
        }
    }

    private IEnumerator Speaking()
    {
        while (true)
        {
            StorySlides[StorySlideNumber].SetActive(false);
            StorySlideNumber++;
            if (StorySlideNumber == StorySlides.Count)
            {
                SceneManager.LoadScene("Final Scene");
            }
            StorySlides[StorySlideNumber].SetActive(true);

            yield return new WaitForSeconds(10); // Wait before repeating
        }
    }
}
