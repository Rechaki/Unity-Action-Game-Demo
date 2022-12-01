using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraData : MonoBehaviour
{
    [SerializeField]
    private Camera characterCamera;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform cameraPivot;
    [SerializeField]
    private float lookSpeed = 0.05f;
    [SerializeField]
    private float followSpeed = 0.1f;
    [SerializeField]
    private float pivotSpeed = 0.03f;

    public Camera CharacterCamera => characterCamera;
    public Transform Target => target;
    public Transform CameraPivot => cameraPivot;
    public float LookSpeed => lookSpeed;
    public float FollowSpeed => followSpeed;
    public float PivotSpeed => pivotSpeed;
}
