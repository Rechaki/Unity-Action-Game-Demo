using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadVFX : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _sprite;
    [SerializeField]
    List<Sprite> _sprites;
    [SerializeField]
    float _speed;

    bool _isShow = false;

    void Update() {
        if (_isShow)
        {
            _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, _sprite.color.a - _speed);
        }
        if (_sprite.color.a == 0)
        {
            _isShow = false;
            gameObject.SetActive(false);
        }
    }

    void OnEnable() {
        int index = Random.Range(0, _sprites.Count);
        _sprite.sprite = _sprites[index];
        _sprite.color = new Color(_sprite.color.r, _sprite.color.g, _sprite.color.b, 1.0f);
        _isShow = true;
    }

    void OnDisable() {
        ObjectPool.I.Recycle(gameObject);
    }
}
