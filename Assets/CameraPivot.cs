using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;

public class CameraPivot : MonoBehaviour
{
    PlayerActions inputActions;
    InputAction cameraMoveAction;

    [SerializeField] private float rotationSpeed = 240;
    [SerializeField] private float rotationAcceleration = 360;

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
    }

    private void OnEnable()
    {
        cameraMoveAction.Enable();
    }
    private void OnDisable()
    {
        cameraMoveAction.Disable();
    }

    public void SetAngle(float angle)
    {
        currentDirection = angle;
    }

    public void SetRotationSpeed(float speed)
    {
        targetRotationSpeed = rotationSpeed * speed;
    }

    public void Update()
    {
        SetRotationSpeed(cameraMoveAction.ReadValue<float>());
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetRotationSpeed, rotationAcceleration * Time.deltaTime);
        currentDirection += currentRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, currentDirection, 0);
        
        Vector3 newPosition = Vector3.Lerp(transform.position, target.position, xLerpSpeed * Time.deltaTime);
        float newYPosition = Mathf.Lerp(transform.position.y, target.position.y, yLerpSpeed * Time.deltaTime);
        transform.position = new Vector3(newPosition.x, newYPosition, newPosition.z);
    }
}
