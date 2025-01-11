using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterModelController : MonoBehaviour
{
    //public GameObject helicopterGroup;
    public GameObject onScreenText;
    
    public HelicopterController[] helicopters;
    private Animator animator;
    private int activeModel = 0;

    private void Start()
    {
        CreateHelicopterArray();
    }

    private void CreateHelicopterArray()
    {
        helicopters = GetComponentsInChildren<HelicopterController>();
        foreach (HelicopterController h in helicopters)
        {
            h.gameObject.SetActive(false);
        }

        helicopters[0].gameObject.SetActive(true);
        animator = helicopters[0].GetComponent<Animator>();
    }

    private void Update()
    {
        CycleModels();
        OpenDoors();
        StartStopEngines();
        HideOnScreenText();
    }

    private void StartStopEngines()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            bool engineState = animator.GetBool("EngineOn");

            if (engineState)
            {
                animator.SetBool("EngineOn", false);
            }
            else
            {
                animator.SetBool("EngineOn", true);
            }
        }
    }

    private void HideOnScreenText()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (onScreenText.active)
            {
                onScreenText.SetActive(false);
            }
            else
            {
                onScreenText.SetActive(true);
            }
        }
    }

    void CycleModels()
    {
        int prevModel = activeModel;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            helicopters[prevModel].gameObject.SetActive(false);
            activeModel++;

            if(activeModel > helicopters.Length - 1)
            {
                activeModel = 0;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            helicopters[prevModel].gameObject.SetActive(false);
            activeModel--;
            if (activeModel < 0)
            {
                activeModel = helicopters.Length - 1;
            }
        }
        
        helicopters[activeModel].gameObject.SetActive(true);
        animator = helicopters[activeModel].gameObject.GetComponent<Animator>();
    }

    void OpenDoors()
    {
        bool frontLeftDoorOpen = animator.GetBool("DoorFrontLeftOpen");
        bool frontRightDoorOpen = animator.GetBool("DoorFrontRightOpen");
        bool rearLeftDoorOpen = animator.GetBool("DoorRearLeftOpen");
        bool rearRightDoorOpen = animator.GetBool("DoorRearRightOpen");

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (frontLeftDoorOpen)
            {
                animator.SetBool("DoorFrontLeftOpen", false);
            }
            else
            {
                animator.SetBool("DoorFrontLeftOpen", true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (frontRightDoorOpen)
            {
                animator.SetBool("DoorFrontRightOpen", false);
            }
            else
            {
                animator.SetBool("DoorFrontRightOpen", true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (rearLeftDoorOpen)
            {
                animator.SetBool("DoorRearLeftOpen", false);
            }
            else
            {
                animator.SetBool("DoorRearLeftOpen", true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (rearRightDoorOpen)
            {
                animator.SetBool("DoorRearRightOpen", false);
            }
            else
            {
                animator.SetBool("DoorRearRightOpen", true);
            }
        }
    }

}
