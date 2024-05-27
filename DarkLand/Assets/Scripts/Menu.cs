using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Add me!!
public class Menu : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene(Settings.ASYLUM_SCENE);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}