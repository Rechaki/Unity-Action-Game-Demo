using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField]
    Transform _pivot;

    void OnEnable() {
        //GlobalMessenger.AddListener(EventMsg.GameClear, () => { ObjectPool.I.Recycle(gameObject); });
        //GlobalMessenger.AddListener(EventMsg.GameOver, () => { ObjectPool.I.Recycle(gameObject); });
    }

    void OnDisable() {
        //GlobalMessenger.RemoveListener(EventMsg.GameClear, () => { ObjectPool.I.Recycle(gameObject); });
        //GlobalMessenger.RemoveListener(EventMsg.GameOver, () => { ObjectPool.I.Recycle(gameObject); });
    }

    void OnDestroy() {
        //GlobalMessenger.RemoveListener(EventMsg.GameClear, () => { ObjectPool.I.Recycle(gameObject); });
        //GlobalMessenger.RemoveListener(EventMsg.GameOver, () => { ObjectPool.I.Recycle(gameObject); });
    }

    public void SetValue(float num)
    {
        if (num <= 0)
        {
            num = 0;
        }
        _pivot.localScale = new Vector3(num, 1, 1);
    }
}
