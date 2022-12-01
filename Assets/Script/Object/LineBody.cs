using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBody : MonoBehaviour
{
    [SerializeField]
    LineRenderer _lineRenderer;
    [SerializeField]
    Transform _targetDirection;
    [SerializeField]
    Transform _wiggleDirection;
    [SerializeField]
    int _length;
    [SerializeField]
    float _targetDistance;
    [SerializeField]
    float _smoothSpeed;
    [SerializeField]
    float _wiggleSpeed;
    [SerializeField]
    float _wiggleMagnitude;
    [SerializeField]
    Vector3[] _segmentPoses;


    Vector3[] _segmentVelocity;

    void Start()
    {
        _lineRenderer.positionCount = _length;
        _segmentPoses = new Vector3[_length];
        _segmentVelocity = new Vector3[_length];
    }

    void Update()
    {
        _wiggleDirection.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(_wiggleSpeed * Time.deltaTime) * _wiggleMagnitude);

        _segmentPoses[0] = _targetDirection.position;

        for (int i = 1; i < _segmentPoses.Length; i++)
        {
            _segmentPoses[i] = Vector3.SmoothDamp(_segmentPoses[i], _segmentPoses[i - 1] + _targetDirection.right * _targetDistance, ref _segmentVelocity[i], _smoothSpeed);
        }
        _lineRenderer.SetPositions(_segmentPoses);
    }
}
