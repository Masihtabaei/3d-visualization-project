using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationMenuManager : MonoBehaviour
{
    [SerializeField]
    private int universitySceneBuildID;

    [SerializeField]
    private int vesteSceneBuildID;

    [SerializeField]
    private int markplatzSceneBuildID;


    public void LoadUniversityScene()
    {
        SceneManager.LoadScene(universitySceneBuildID);
    }

    public void LoadVesteScene()
    {
        SceneManager.LoadScene(vesteSceneBuildID);
    }

    public void LoadMarkplatzScene()
    {
        SceneManager.LoadScene(markplatzSceneBuildID);
    }
}
