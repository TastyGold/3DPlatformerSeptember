using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.DefaultInputActions;
using UnityEngine.InputSystem;
using UnityEngine.SearchService;

public class PlayerMove : MonoBehaviour
{
    private PlayerActions playerActions;
    private InputAction jumpAction;
    private InputAction moveAction;

    private CameraPivot cameraPivot;
    private FloorDetection floorDetection;

    [SerializeField] private float jumpVelocity = 3;
    [SerializeField] private float jumpHoverDuration = 0.5f;
    [SerializeField] private float jumpRiseGravity = 5;
    [SerializeField] private float jumpFallGravity = 7;
    [SerializeField] private float terminalVelocity = 5;
    [SerializeField] private float groundedHeight = 0;
    [SerializeField] private float walkSpeed = 10;
    private Vector3 velocity;
    private States currentState = States.Grounded;
    private float jumpHoverTimer = 0;
    private Rigidbody rb;

    enum States
    {
        Grounded,
        JumpRise,
        JumpHover,
        JumpFall
    }

    void Awake()
    {
        playerActions = new PlayerActions();
        jumpAction = playerActions.PlayerControls.Jump;
        //jumpAction.performed += OnJumpStarted;
        //jumpAction.canceled += OnJumpEnded;
        moveAction = playerActions.PlayerControls.Move;
        rb = GetComponent<Rigidbody>();
        cameraPivot = FindObjectOfType<CameraPivot>();
        floorDetection = GetComponentInChildren<FloorDetection>();
    }

    void OnEnable()
    {
        jumpAction.Enable();
        moveAction.Enable();
    }
    void OnDisable()
    {
        jumpAction.Disable();
        moveAction.Disable();
    }
    void Start()
    {
    }

    //void OnJumpStarted(InputAction.CallbackContext context)
    //{
    //}

    //void OnJumpEnded(InputAction.CallbackContext context)
    //{
    //}

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("trigger!");
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        HandleWalking();
        HandleJumping();
        rb.velocity = velocity;
    }

    private void HandleWalking()
    {
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Vector3 rotatedMovementInput = Quaternion.AngleAxis(cameraPivot.CurrentDirection, Vector3.up) * new Vector3(movementInput.x, 0, movementInput.y);
        //Debug.Log(cameraPivot.CurrentDirection);
        float yVel = velocity.y;
        velocity = walkSpeed * rotatedMovementInput;
        velocity.y = yVel;
    }

    private bool IsGrounded()
    {
        return floorDetection.Floors > 0;
    }

    private void HandleJumping()
    {
        switch (currentState)
        {
            case States.Grounded:
                {
                    if (jumpAction.IsPressed())
                    {
                        velocity.y = jumpVelocity;
                        currentState = States.JumpRise;
                    }
                    else if (!IsGrounded())
                    {
                        currentState = States.JumpFall;
                    }
                    break;
                }
            case States.JumpRise:
                {
                    velocity.y -= jumpRiseGravity * Time.fixedDeltaTime;
                    if (velocity.y <= 0 && jumpAction.IsPressed())
                    {
                        velocity.y = 0;
                        currentState = States.JumpHover;
                        jumpHoverTimer = jumpHoverDuration;
                    }
                    else if (!jumpAction.IsPressed())
                    {
                        currentState = States.JumpFall;
                        velocity *= 0.5f;
                    }
                    break;
                }
            case States.JumpHover:
                {
                    jumpHoverTimer -= Time.fixedDeltaTime;
                    if (jumpHoverTimer <= 0 || !jumpAction.IsPressed())
                    {
                        currentState = States.JumpFall;
                    }
                    break;
                }
            case States.JumpFall:
                {
                    velocity.y -= jumpFallGravity * Time.fixedDeltaTime;
                    if (velocity.y <= -terminalVelocity)
                    {
                        velocity.y = -terminalVelocity;
                    }
                    if (IsGrounded())
                    {
                        velocity.y = 0;
                        currentState = States.Grounded;
                        //transform.position = new Vector3(transform.position.x, groundedHeight, transform.position.z);
                    }
                    break;
                }
        }
    }

    private void HandleCameraMovement()
    {

    }
}
