using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartExit : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("Final Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
