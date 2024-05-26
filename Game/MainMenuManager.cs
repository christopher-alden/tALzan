using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject Settings;
    public GameObject Main;
    public bool isInSettings;

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("ok play game");
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Loading");
    }

    public void GameSettings()
    {
        isInSettings = !isInSettings;
        if (isInSettings)
        {
            Settings.gameObject.SetActive(true);
            Main.gameObject.SetActive(false);
        }
        else
        {
            Settings.gameObject.SetActive(false);
            Main.gameObject.SetActive(true);
        }

    }

    public void SetAntiAliasing(int level)
    {
        if(level != 0)
        {
            level = (int)Mathf.Pow(2, level);
        }
        QualitySettings.antiAliasing = level;
    }

    public void SetQuality(int level)
    {
        QualitySettings.SetQualityLevel(level);
    }

    public void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }
}
