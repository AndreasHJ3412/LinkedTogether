using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueButtton : MonoBehaviour
{
    public List<GameObject> Scenes;

    public int SceneNumber;

    public float Timer;

    private void Start()
    {
        doit();
    }

    public void OnButtonPressed()
    {
        doit();
    }

    private void Update()
    {
        if (Timer > 5)
        {
            doit();
        }
        else
        {
            Timer += Time.deltaTime;
        }
    }

    void doit()
    {
        Timer = 0;

        Scenes[SceneNumber].SetActive(false);
        SceneNumber++;
        if (SceneNumber == Scenes.Count)
        {
            SceneManager.LoadScene("Final Scene");
        }
        Scenes[SceneNumber].SetActive(true);
    }
}
