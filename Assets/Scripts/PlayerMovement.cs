using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, ISpeedProvider
{
    [Header("Speed")]
    public float acceleration = 2f;
    public float breakSpeed = 4f;
    public float currentSpeed;
    public float CurrentSpeed => currentSpeed;

    [Header("Gas")]
    //public float gas = 100f;

    [Header("Lane Check")]
    public Transform laneCheck;
    public float laneCheckRadius = 0.2f;
    public LayerMask laneLayer;
    
    private Rigidbody2D rb;
    private float moveInput;
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
        //passedLane = Physics2D.OverlapCircle(laneCheck.position, laneCheckRadius, laneLayer);
        moveInput = Keyboard.current.wKey.isPressed ? 1f : Keyboard.current.sKey.isPressed ? -1f : 0f;
        if (moveInput != 0f) 

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
        rb.AddForce(transform.up * acceleration * moveInput * Time.deltaTime);
        if (isBreaking && rb.linearVelocity.magnitude > 0.1f) {
            rb.AddForce(rb.linearVelocity.normalized * -1 * Time.deltaTime * breakSpeed);
        } else if (isBreaking) {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        currentSpeed = rb.linearVelocity.magnitude;
    }
}