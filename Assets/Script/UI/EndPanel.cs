using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanel : MonoBehaviour
{
    void Start()
    {
        LevelManager.I.LoadScene("Title");
    }
}
