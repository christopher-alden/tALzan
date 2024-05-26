using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    //public GameObject loadingScreen;
    public Slider loadingProgressSlider;
    public string sceneNameToLoad;

    private void Start()
    {
        //loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneNameToLoad));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        sceneLoad.allowSceneActivation = false;

        while (!sceneLoad.isDone)
        {
            loadingProgressSlider.value = sceneLoad.progress;

            if (sceneLoad.progress >= 0.9f)
            {
                sceneLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
