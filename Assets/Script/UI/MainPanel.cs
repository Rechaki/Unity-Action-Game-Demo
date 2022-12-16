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

    Vector2 pos = Vector2.zero;
    Vector2 _lockonTargetPos;
    bool _isLockon = false;

    void Start()
    {
        GlobalMessenger<Vector2>.AddListener(EventMsg.LockOnEnter, ShowLockonArrow);
        GlobalMessenger.AddListener(EventMsg.LockOnExit, HideLockonArrow);
    }

    void Update()
    {
        UpdateLockonArrowPosition();
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
            _isLockon = false;
        }
    }

    void UpdateLockonArrowPosition()
    {
        if (_lockonArrow.gameObject.activeSelf)
        {
            _lockonTargetPos.x -= Screen.width / 2;
            _lockonTargetPos.y -= Screen.height / 2;
            _lockonArrow.localPosition = _lockonTargetPos;
        }
    }
}
