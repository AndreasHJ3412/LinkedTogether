using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPlatform : MonoBehaviour
{
    public bool GodSpeaking;
    public GameObject GodSpeaking_Canvas;

    public List<GameObject> StorySlides;
    private int StorySlideNumber = -1;

    public bool once;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GodSpeaking == false)
        {

            if (collision.gameObject.CompareTag("Player"))
            {
                GodSpeaking = true;
                StartCoroutine(Speaking());
            }

        }
    }

    private IEnumerator Speaking()
    {
        while (true)
        {
            if (StorySlideNumber >= 0)
            {
                StorySlides[StorySlideNumber].SetActive(false);
            }

            StorySlideNumber++;
            if (StorySlideNumber == StorySlides.Count)
            {
                SceneManager.LoadScene("MainMenu");
            }
            StorySlides[StorySlideNumber].SetActive(true);

            yield return new WaitForSeconds(5); // Wait before repeating
        }
    }
}
