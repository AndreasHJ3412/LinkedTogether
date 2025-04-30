using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueButtton : MonoBehaviour
{
    public List<GameObject> Scenes;

    public int SceneNumber;

    public void OnButtonPressed()
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
