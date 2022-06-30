using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float cameraMoveSpeed = 10f;
    [SerializeField] float cameraRotationSpeed = 100f;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] float zoomAmount = 1f;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachineTransposer cinemachineTransposer;
    Vector3 targetFollowOffset;
    const float Min_Follow_Y_Limit = 2f;
    const float Max_Follow_Y_Limit = 12f;


    private void Awake()
    {
       cinemachineTransposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
       targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);

        rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

        transform.eulerAngles += rotationVector * cameraRotationSpeed * Time.deltaTime;
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * cameraMoveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomIncreaseAmount = 1f;
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, Min_Follow_Y_Limit, Max_Follow_Y_Limit);
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }
}
