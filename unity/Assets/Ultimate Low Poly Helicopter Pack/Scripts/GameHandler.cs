using System.Collections;
using UnityEngine;

namespace Assets
{
    public class GameHandler : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ScreenshotHandler.TakeScreenshot_Static(Screen.width,Screen.height);

            }
        }
    }
}