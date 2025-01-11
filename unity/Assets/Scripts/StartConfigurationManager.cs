using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartConfigurationManager : MonoBehaviour
{

    [SerializeField]
    private TMP_Dropdown controller;

    [SerializeField]
    private TMP_InputField serverAddress;

    [SerializeField]
    private Slider progressBar;

    private int selectedSceneIndex = 1;

    public void SetLocation(int index) 
    { 
        selectedSceneIndex = 1;
    }

    public void StartSimulation() 
    {
        Debug.Log(serverAddress.text);
        Debug.Log(controller.value);
        StartCoroutine(LoadSceneAsynchronously());
    }

    IEnumerator LoadSceneAsynchronously()
    { 
        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(selectedSceneIndex);
        while (!sceneLoadingOperation.isDone) 
        {
            float loadingProgress = Mathf.Clamp01(sceneLoadingOperation.progress / 0.9f);
            Debug.Log(loadingProgress);
            progressBar.value = loadingProgress;
            yield return null;
        }
    }

}
