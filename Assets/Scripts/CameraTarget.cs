using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private static float screenResolution, inverseScreenResolution;
    [SerializeField] private Transform _player;
    [SerializeField] private float _cameraDistance = 1f;
    [SerializeField] float minimumViewportPoint = 0.1f, maximumViewportPoint = 0.9f;
    private Vector3 _target = Vector3.zero;
    private Vector3 _mousePos = Vector3.zero;
    private Vector2 _halfUnitVector = Vector2.one / 2;
    private Vector3 _refVel;

    private void Start()
    {
        screenResolution = (((float)Screen.width / Screen.height) + 1) / 2;
        inverseScreenResolution = (((float)Screen.height / Screen.width) + 1) / 2;
    }

    private void FixedUpdate()
    {
        _mousePos = CaptureMousePos();
        _target = UpdateTargetPos();
        transform.position = _target;
    }

    Vector3 CaptureMousePos()
    {
        Vector2 cameraPosViewport = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        cameraPosViewport.x = Mathf.Clamp(cameraPosViewport.x, minimumViewportPoint, maximumViewportPoint);
        cameraPosViewport.y = Mathf.Clamp(cameraPosViewport.y, minimumViewportPoint, maximumViewportPoint);

        cameraPosViewport -= _halfUnitVector;

        cameraPosViewport.x *= screenResolution;
        cameraPosViewport.y *= inverseScreenResolution;

        return cameraPosViewport;
    }

    Vector3 UpdateTargetPos()
    {
        Vector3 mouseOffset = _mousePos * _cameraDistance;
        Vector3 updatedTargetPos = _player.position + mouseOffset;
        updatedTargetPos.z = 0;

        return updatedTargetPos;
    }
}
