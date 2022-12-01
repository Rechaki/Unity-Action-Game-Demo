using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField]
    private Transform characterModel;
    [SerializeField]
    private Animator characterAnimator;
    [SerializeField]
    private Rigidbody characterRigidbody;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float moveSmoothingSpeed = 2f;
    [SerializeField]
    private float turnSpeed = 0.3f;
    [SerializeField]
    private float height;
    [SerializeField]
    private float distance;
    [SerializeField]
    private float rotateSpeed;

    public Transform CharacterModel => characterModel;
    public Rigidbody CharacterRigidbody => characterRigidbody;
    public Animator CharacterAnimator => characterAnimator;
    public float MoveSpeed => moveSpeed;
    public float MoveSmoothingSpeed => moveSmoothingSpeed;
    public float TurnSpeed => turnSpeed;

}
