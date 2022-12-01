using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float timeScale = 1.0f;

    Rigidbody2D _rb;

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 v) {
        float x = v.x;
        float y = v.y;
        if (x * y != 0)
        {
            x = x * Mathf.Sqrt(1 - (y * y) / 2.0f);
            y = y * Mathf.Sqrt(1 - (x * x) / 2.0f);
        }
        Vector3 moveInput = new Vector3(x, y, 0);
        _rb.velocity = moveInput * moveSpeed * timeScale;

        if (x > 0)
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        else if(x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
