using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ICharacter
{
    public Transform lookatTarget => _lookatTarget;

    [SerializeField]
    Transform _lookatTarget;



}
