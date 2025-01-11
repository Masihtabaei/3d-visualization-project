using UnityEngine;

public class AirplaneCameraController : MonoBehaviour
{

    [SerializeField]
    private Transform[] pointOfViews;
    
    [SerializeField]
    private float speed = 1000;

    private int pointOfViewIndex = 0;
    private Vector3 target;


    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) pointOfViewIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) pointOfViewIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) pointOfViewIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) pointOfViewIndex = 3;

        target = pointOfViews[pointOfViewIndex].position;

        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
        transform.forward = pointOfViews[pointOfViewIndex].forward;

    }
}
