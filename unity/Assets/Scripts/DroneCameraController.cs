using UnityEngine;

public class DroneCameraController:MonoBehaviour {
    public float movementSpeed = 10.0f;
    public float rotationSpeed = 2.0f;

    public float minYRotation = -90f;
    public float maxYRotation = 90f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        MoveCamera();
        RotateCameraWithMouse();
    }

    void MoveCamera() {
        float moveForward = Input.GetAxis("Pitch") * movementSpeed * Time.deltaTime;
        float moveSideways = Input.GetAxis("Roll") * movementSpeed * Time.deltaTime;

        float moveUpDown = 0f;
        if (Input.GetKey(KeyCode.Space)) moveUpDown += movementSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift)) moveUpDown -= movementSpeed * Time.deltaTime;

        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        Vector3 movement = forward * moveForward + right * moveSideways + Vector3.up * moveUpDown;

        transform.Translate(movement,Space.World);
    }

    void RotateCameraWithMouse() {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        rotationX += mouseX;
        rotationY -= mouseY;

        rotationY = Mathf.Clamp(rotationY,minYRotation,maxYRotation);

        transform.rotation = Quaternion.Euler(rotationY,rotationX,0);
    }
}
