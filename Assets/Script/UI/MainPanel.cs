using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    [SerializeField]
    Camera uiCamera;
    [SerializeField]
    RectTransform _lockonArrow;

    Vector2 _lockonTargetPos;
    bool _isLockon = false;

    void Start()
    {
        GlobalMessenger<Vector2>.AddListener(EventMsg.LockOnEnter, ShowLockonArrow);
        GlobalMessenger.AddListener(EventMsg.LockOnExit, HideLockonArrow);
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        GlobalMessenger<Vector2>.RemoveListener(EventMsg.LockOnEnter, ShowLockonArrow);
        GlobalMessenger.AddListener(EventMsg.LockOnExit, HideLockonArrow);
    }

    void ShowLockonArrow(Vector2 v)
    {
        if (_lockonArrow.gameObject.activeSelf == false)
        {
            _lockonArrow.gameObject.SetActive(true);
        }

        _lockonTargetPos = v;
        _isLockon = true;
    }

    void HideLockonArrow()
    {
        if (_lockonArrow.gameObject.activeSelf)
        {
            _lockonArrow.gameObject.SetActive(false);
        }
        _isLockon = false;
    }

    void UpdateLockonArrowPosition()
    {
        if (_lockonArrow.gameObject.activeSelf)
        {
            Vector2 pos = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_lockonArrow, Vector2.zero, uiCamera, out pos))
            {
                _lockonArrow.localPosition = pos;
                Debug.Log(pos);
            }
        }
    }
}
