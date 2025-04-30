using System.Collections.Generic;
using UnityEngine;

public class DialogueButtton : MonoBehaviour
{
    public List<GameObject> Scenes;

    public int SceneNumber;

    public void OnButtonPressed()
    {
        Scenes[SceneNumber].SetActive(false);
        SceneNumber++;
        Scenes[SceneNumber].SetActive(true);
    }
}
