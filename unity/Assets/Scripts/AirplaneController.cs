using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    [SerializeField]
    private float throttleIncrementStep = 0.1f;

    [SerializeField]
    private float throttleMaximumValue = 200;

    [SerializeField]
    private float responsiveness = 10f;

    [SerializeField]
    private float lift = 150f;

    [SerializeField]
    private Transform propella;

    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;

    private Rigidbody body;
    private AudioSource engineSound;

    private float responseModifier {
        get { 
            return (body.mass * 10f) / responsiveness;
        }
    }


    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
    }

    private void HandleInput() {
        roll = Input.GetAxis("Roll"); 
        pitch = Input.GetAxis("Pitch"); 
        yaw = Input.GetAxis("Yaw");

        if (Input.GetKey(KeyCode.Space)) throttle += throttleIncrementStep;
        else if (Input.GetKey(KeyCode.LeftControl)) throttle -= throttleIncrementStep;

        throttle = Mathf.Clamp(throttle, 0f, 100f);
    }

    private void Update() { 
        HandleInput();
        RotatePropella();
        engineSound.volume = throttle * 0.1f;
    }

    private void RotatePropella() 
    {
        propella.Rotate(Vector3.forward * throttle);
    }
    private void FixedUpdate()
    {
        body.AddForce(transform.forward * throttleMaximumValue * throttle);
        body.AddTorque(transform.up * yaw * responseModifier);
        body.AddTorque(transform.right * pitch * responseModifier);
        body.AddTorque(- transform.forward * roll * responseModifier);

        body.AddForce(Vector3.up * body.linearVelocity.magnitude * lift);
    }

}
