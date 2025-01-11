using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartConfigurationManager : MonoBehaviour
{
    [SerializeField]
    private int universitySceneBuildID;

    [SerializeField]
    private int vesteSceneBuildID;

    [SerializeField]
    private int markplatzSceneBuildID;

    [SerializeField]
    private TMP_Dropdown controller;

    [SerializeField]
    private TMP_InputField serverAddress;

    private int selectedSceneIndex;

    public void SetLocation(int index) 
    { 
        selectedSceneIndex = index;
    }

    public void StartSimulation() 
    {
        Debug.Log(serverAddress.text);
        Debug.Log(controller.value);
    }
    private void LoadUniversityScene()
    {
        SceneManager.LoadScene(universitySceneBuildID);
    }

    private void LoadVesteScene()
    {
        SceneManager.LoadScene(vesteSceneBuildID);
    }

    private void LoadMarkplatzScene()
    {
        SceneManager.LoadScene(markplatzSceneBuildID);
    }
}
