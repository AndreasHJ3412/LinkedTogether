using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueButtton : MonoBehaviour
{
    public List<GameObject> Scenes;

    public int SceneNumber;

    private void Start()
    {
        StartCoroutine(Speaking());
    }

    public void OnButtonPressed()
    {
        doit();
    }

    private IEnumerator Speaking()
    {
        while (true)
        {
            doit();

            yield return new WaitForSeconds(10); // Wait before repeating
        }
    }

    void doit()
    {
        Scenes[SceneNumber].SetActive(false);
        SceneNumber++;
        if (SceneNumber == Scenes.Count)
        {
            SceneManager.LoadScene("Final Scene");
        }
        Scenes[SceneNumber].SetActive(true);
    }
}
