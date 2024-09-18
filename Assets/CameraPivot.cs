using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class CameraPivot : MonoBehaviour
{
    PlayerActions inputActions;
    InputAction cameraMoveAction;
    InputAction playerMoveAction;

    [SerializeField] private float rotationSpeed = 240;
    [SerializeField] private float rotationAcceleration = 360;

    [SerializeField] private float strafeCameraTurnSpeed = 50;
    [SerializeField] private float strafeDeadzone = 0.5f;

    [SerializeField] private float xLerpSpeed = 10;
    [SerializeField] private float yLerpSpeed = 5;

    [SerializeField] private Transform target;

    private float targetRotationSpeed;
    private float currentRotationSpeed;
    private float currentDirection = 0;
    public float CurrentDirection
    {
        get
        {
            return currentDirection;
        }
    }

    private void Awake()
    {
        inputActions = new PlayerActions();
        cameraMoveAction = inputActions.CameraControls.Rotate;
        playerMoveAction = inputActions.PlayerControls.Move;
    }

    private void OnEnable()
    {
        cameraMoveAction.Enable();
        playerMoveAction.Enable();
    }
    private void OnDisable()
    {
        cameraMoveAction.Disable();
        playerMoveAction.Disable();
    }

    public void SetAngle(float angle)
    {
        currentDirection = angle;
    }

    public void SetRotationSpeed(float speed, float strafe)
    {
        float deadzoneStrafe = Mathf.Abs(strafe);
        deadzoneStrafe -= strafeDeadzone;
        deadzoneStrafe = Mathf.Max(deadzoneStrafe, 0);
        deadzoneStrafe *= Mathf.Sign(strafe);
        deadzoneStrafe /= 1 - strafeDeadzone;

        targetRotationSpeed = rotationSpeed * speed + deadzoneStrafe * strafeCameraTurnSpeed;
    }

    public void FixedUpdate()
    {
        SetRotationSpeed(cameraMoveAction.ReadValue<float>(), playerMoveAction.ReadValue<Vector2>().x);
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetRotationSpeed, rotationAcceleration * Time.fixedDeltaTime);
        currentDirection += currentRotationSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(0, currentDirection, 0);
        
        Vector3 newPosition = Vector3.Lerp(transform.position, target.position, xLerpSpeed * Time.fixedDeltaTime);
        float newYPosition = Mathf.Lerp(transform.position.y, target.position.y, yLerpSpeed * Time.fixedDeltaTime);
        transform.position = new Vector3(newPosition.x, newYPosition, newPosition.z);
    }
}
