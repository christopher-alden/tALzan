using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        
    }
    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
