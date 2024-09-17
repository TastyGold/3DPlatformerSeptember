using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.DefaultInputActions;
using UnityEngine.InputSystem;
using UnityEngine.SearchService;

public class PlayerMove : MonoBehaviour
{
    PlayerActions playerActions;
    InputAction jumpAction;
    InputAction moveAction;

    private CameraPivot cameraPivot;

    [SerializeField] private float jumpVelocity = 3;
    [SerializeField] private float jumpHoverDuration = 0.5f;
    [SerializeField] private float jumpRiseGravity = 5;
    [SerializeField] private float jumpFallGravity = 7;
    [SerializeField] private float terminalVelocity = 5;
    [SerializeField] private float groundedHeight = 0;
    [SerializeField] private float walkSpeed = 10;
    private float velocity = 0;
    private States currentState = States.Grounded;
    private float jumpHoverTimer = 0;
    private Rigidbody2D rb;

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
        rb = GetComponent<Rigidbody2D>();
        cameraPivot = FindObjectOfType<CameraPivot>();
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

    void Update()
    {
        HandleWalking();
        HandleJumping();
    }

    private void HandleWalking()
    {
        Vector2 movementInput = moveAction.ReadValue<Vector2>();
        Vector3 rotatedMovementInput = Quaternion.AngleAxis(cameraPivot.CurrentDirection, Vector3.up) * new Vector3(movementInput.x, 0, movementInput.y);
        Debug.Log(cameraPivot.CurrentDirection);
        transform.position += Time.deltaTime * walkSpeed * rotatedMovementInput;
    }

    private void HandleJumping()
    {
        switch (currentState)
        {
            case States.Grounded:
                {
                    if (jumpAction.IsPressed())
                    {
                        velocity = jumpVelocity;
                        currentState = States.JumpRise;
                    }
                    break;
                }
            case States.JumpRise:
                {
                    transform.Translate(Time.deltaTime * velocity * Vector3.up);
                    velocity -= jumpRiseGravity * Time.deltaTime;
                    if (velocity <= 0 && jumpAction.IsPressed())
                    {
                        velocity = 0;
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
                    jumpHoverTimer -= Time.deltaTime;
                    if (jumpHoverTimer <= 0 || !jumpAction.IsPressed())
                    {
                        currentState = States.JumpFall;
                    }
                    break;
                }
            case States.JumpFall:
                {
                    transform.Translate(Time.deltaTime * velocity * Vector3.up);
                    velocity -= jumpFallGravity * Time.deltaTime;
                    if (velocity <= -terminalVelocity)
                    {
                        velocity = -terminalVelocity;
                    }
                    if (transform.position.y <= groundedHeight)
                    {
                        velocity = 0;
                        currentState = States.Grounded;
                        transform.position = new Vector3(transform.position.x, groundedHeight, transform.position.z);
                    }
                    break;
                }
        }
    }

    private void HandleCameraMovement()
    {

    }
}
