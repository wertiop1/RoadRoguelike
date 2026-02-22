using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, ISpeedProvider
{
    [Header("Speed")]
    public float acceleration = 50f;
    public float turnSpeed = 20f;
    public float breakSpeed = 50f;
    public float speedLimit = 10f;    
    public float currentSpeed;
    public float CurrentSpeed => currentSpeed;

    [Header("Lane Check")]
    public Transform laneCheck;
    public float laneCheckRadius = 0.2f;
    public LayerMask laneLayer;
    
    private Rigidbody2D rb;
    private float moveInput;
    private float turnInput;
    private bool passedLane;

    private float blinker;

    private bool isBreaking;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.mass = 0.2f;
        blinker = 0f;
        isBreaking = false;
    }

    void Update()
    {   
        moveInput = Keyboard.current.wKey.isPressed ? 1f : Keyboard.current.sKey.isPressed ? -1f : 0f;
        turnInput = Keyboard.current.aKey.isPressed ? 1f : Keyboard.current.dKey.isPressed ? -1f : 0f;

        if (Keyboard.current.qKey.isPressed) {
            Debug.Log("Left Blinker");
            blinker = 1f;
        } else if (Keyboard.current.eKey.isPressed) {
            Debug.Log("Right Blinker");
            blinker = -1f;
        }

        if (Keyboard.current.leftShiftKey.isPressed) {
            isBreaking = true;
        } else {
            isBreaking = false;
        }
    }

    void FixedUpdate()
    {   
        // Rotate the car
        transform.Rotate(0f, 0f, turnInput * turnSpeed * Time.deltaTime);

        // Accelerate along the car's forward direction
        rb.AddForce(transform.up * acceleration * moveInput * Time.deltaTime);

        // Brake
        if (isBreaking && rb.linearVelocity.magnitude > 0.1f) {
            rb.AddForce(rb.linearVelocity.normalized * -breakSpeed * Time.deltaTime);
        } else if (isBreaking) {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Align velocity with car rotation (so it always goes forward)
        if (rb.linearVelocity.magnitude > 0.01f) {
            if(rb.linearVelocity.magnitude > 10)
            {
                rb.linearVelocity = speedLimit * (Vector2)transform.up;
            }
            else rb.linearVelocity = rb.linearVelocity.magnitude * (Vector2)transform.up;
        }
        currentSpeed = rb.linearVelocity.magnitude;
    }
}