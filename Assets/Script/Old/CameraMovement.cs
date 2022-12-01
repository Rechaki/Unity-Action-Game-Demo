using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    float lookAngle;
    float pivotAngle;
    float displaceAngle;
    float defaultPositionZ;
    bool isRotate = false;


    Vector2 inputStick;
    CameraData cameraData;
    InputData inputData;
    Transform cameraTransform;

    const float minPivot = -30f;
    const float maxPivot = 30f;
    const float defaulOffset = 3.6f;
    const float sphereAdius = 0.2f;
    const float minOffset = 0.8f;

    void Awake()
    {
        inputData = GameDataManagement.I.InputData;
        cameraData = GameDataManagement.I.CameraData;
        cameraTransform = cameraData.CharacterCamera.transform;
        defaultPositionZ = cameraTransform.position.z;
    }

    void FixedUpdate()
    {
        if (inputData.StickRightValue == Vector2.zero) {
            inputStick = inputData.StickLeftValue;
            isRotate = false;
        } else {
            inputStick = inputData.StickRightValue;
            isRotate = true;
        }
        
        FollowTarget();
        if (isRotate) {
            OnRotation(inputStick);
        } else {
            RotateWithMove(inputStick);
        }

    }

    private void FollowTarget()
    {
        Vector3 targetPos = Vector3.Lerp(transform.position, cameraData.Target.position, Time.deltaTime / cameraData.FollowSpeed);
        transform.position = targetPos;
        CollisionToObject();
    }

    private void RotateWithMove(Vector2 direction)
    {
        lookAngle += direction.x * cameraData.LookSpeed / Time.deltaTime;
        Vector3 rotation = new Vector3(0, lookAngle, 0);
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        //if (cameraPivot.localRotation.x != 0) {
            //m_pivotAngle = 0;
            //cameraPivot.localRotation = Quaternion.Slerp(cameraPivot.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime);
            //cameraPivot.localRotation = Quaternion.Euler(Vector3.zero);
            //cameraPivot.localRotation = Quaternion.identity;
        //}
    }

    private void OnRotation(Vector2 direction)
    {
        lookAngle += direction.x * cameraData.LookSpeed / Time.deltaTime;
        pivotAngle -= direction.y * cameraData.LookSpeed / Time.deltaTime;
        pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);

        Vector3 rotation = new Vector3(0, lookAngle, 0);
        transform.rotation = Quaternion.Euler(rotation);

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        
        cameraData.CameraPivot.localRotation = Quaternion.Euler(rotation);
    }

    private void CollisionToObject()
    {
        Vector3 direction = cameraTransform.position - cameraData.CameraPivot.position;
        direction.Normalize();
        float targetPositionZ = defaultPositionZ;
        
        RaycastHit hitInfo;
        if (Physics.SphereCast(cameraData.CameraPivot.position, sphereAdius, direction, 
                                out hitInfo, defaulOffset, LayerMask.GetMask("Terrain"))) {
            float dis = Vector3.Distance(cameraData.CameraPivot.position, hitInfo.point);
            targetPositionZ = -(dis - sphereAdius);
        }
        if (Mathf.Abs(targetPositionZ) < minOffset) {
            targetPositionZ = -minOffset;
        }
        Vector3 targetPos = new Vector3(cameraTransform.localPosition.x, cameraTransform.localPosition.y, targetPositionZ);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, Time.deltaTime / 0.2f);
        
    }

}
