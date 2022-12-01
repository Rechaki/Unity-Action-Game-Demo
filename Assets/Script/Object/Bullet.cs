using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public UnitData owner;
    public SkillData skillData;
    public Vector3 moveDirection;
    public Vector3 target;
    public float speed;
    public int damage;

    float _lifeTime = 0.5f;
    float _timer;

    void OnEnable() {
        _timer = 0;    
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (moveDirection == Vector3.zero)
            {
                _timer += Time.deltaTime;
                if (_timer > _lifeTime)
                {
                    _timer = 0;
                    ObjectPool.I.Recycle(gameObject);
                }
            }
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (gameObject.tag == "Bullet")
        {
            if (collision.tag == "Enemy" || collision.tag == "Untagged")
            {
                ObjectPool.I.Recycle(gameObject);
            }
        }
        else if (gameObject.tag == "EnemyBullet")
        {
            if (collision.tag == "Player" || collision.tag == "Untagged")
            {
                ObjectPool.I.Recycle(gameObject);
            }
        }
    }

    void Recycle() {
        ObjectPool.I.Recycle(gameObject);
    }

}
