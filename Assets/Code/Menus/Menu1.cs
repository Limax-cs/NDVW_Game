using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu1 : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("TestWorld");
    }

    public void ControlsButton()
    {
    }

    public void GameConfigurationButton()
    {
    }

    public void Quit()
    {
        Application.Quit();
    }
}
