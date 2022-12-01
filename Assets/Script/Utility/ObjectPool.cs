using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private Dictionary<int, Queue<GameObject>> _poolDic = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<GameObject, int> _poppedIdDic = new Dictionary<GameObject, int>();

    public void ClearCachePool() {
        foreach (var item in _poppedIdDic)
        {
            _poolDic[item.Value].Enqueue(item.Key);
        }
        foreach (var item in _poolDic)
        {
            while(item.Value.Count > 0)
            {
                var go = item.Value.Dequeue();
                Destroy(go);
            }

            item.Value.Clear();
        }
        _poolDic.Clear();
        _poppedIdDic.Clear();
    }

    public GameObject Create(GameObject prefab) {
        int id = prefab.GetInstanceID();
        GameObject go = null;
        Queue<GameObject> objects;
        if (_poolDic.TryGetValue(id, out objects) && objects.Count > 0)
        {
            go = objects.Dequeue();
        }
        else
        {
            if (objects == null)
            {
                objects = new Queue<GameObject>();
                _poolDic.Add(id, objects);
            }

            if (objects.Count == 0)
            {
                go = Instantiate<GameObject>(prefab);
            }
            //objects.Enqueue(go);
        }

        go.transform.SetParent(transform);
        _poppedIdDic.Add(go, id);
        return go;
    }

    public void Recycle(GameObject go) {
        if (go == null)
        {
            return;
        }

        go.SetActive(false);
        go.transform.SetParent(transform);

        int id;
        if (_poppedIdDic.TryGetValue(go, out id))
        {
            _poppedIdDic.Remove(go);
            Queue<GameObject> objects;
            if (_poolDic.TryGetValue(id, out objects))
            {
                objects.Enqueue(go);
                
            }
            else
            {
                _poolDic[id] = new Queue<GameObject>();
                _poolDic[id].Enqueue(go);
            }

        }
    }

}
