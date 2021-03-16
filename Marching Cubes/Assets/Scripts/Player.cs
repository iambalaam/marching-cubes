using UnityEngine;

public class Player : MonoBehaviour
{
    // Looking
    private float MOUSE_SENSITIVITY = 100f;
    private float MOUSE_SMOOTHING = 0.5f;
    private float xRotation = 90f;
    private float yRotation = 0f;

    // Moving
    private float MOVE_FORCE = 500f;
    private bool thrust;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        if (thrust)
        {
            rb.AddForce(transform.forward * MOVE_FORCE * Time.fixedDeltaTime);
        }
    }

    void PlayerInput()
    {
        // Looking
        float mouseX = Input.GetAxis("Mouse X") * MOUSE_SENSITIVITY * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MOUSE_SENSITIVITY * Time.deltaTime;

        xRotation = Mathf.Clamp(xRotation - mouseY, -90, 90);
        yRotation += mouseX;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(xRotation, yRotation, 0), MOUSE_SMOOTHING); ;

        // Moving
        thrust = Input.GetKey(KeyCode.W);
    }
}
