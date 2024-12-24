using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // Nur für den Editor verfügbar
#endif

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
        // Bewegen und Drehen der Kamera
        MoveCamera();
        RotateCameraWithMouse();

        // Play-Modus beenden, wenn Escape gedrückt wird (nur im Editor)
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape)) {
            EditorApplication.isPlaying = false;
        }
#endif
    }

    void MoveCamera() {
        // Vorwärts- und Seitwärtsbewegung berechnen
        float moveForward = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float moveSideways = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        // Vertikale Bewegung für Auf- und Absteigen
        float moveUpDown = 0f;
        if (Input.GetKey(KeyCode.Space)) moveUpDown += movementSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift)) moveUpDown -= movementSpeed * Time.deltaTime;

        // Bewegung in die horizontale Ebene umsetzen
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        Vector3 movement = forward * moveForward + right * moveSideways + Vector3.up * moveUpDown;

        // Bewegung anwenden
        transform.Translate(movement,Space.World);
    }

    void RotateCameraWithMouse() {
        // Mausbewegungen für Rotation erfassen
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Rotation um die X- und Y-Achse berechnen
        rotationX += mouseX;
        rotationY -= mouseY;

        // Begrenzung der vertikalen Rotation (Pitch)
        rotationY = Mathf.Clamp(rotationY,minYRotation,maxYRotation);

        // Rotation anwenden
        transform.rotation = Quaternion.Euler(rotationY,rotationX,0);
    }
}
