using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosePanel : UIPanel
{
    [SerializeField]
    ToggleGroup _toggleGroup;
    [SerializeField]
    Button _startBtn;

    List<HeroChooseButton> _chooseBtns;
    int _index = 0;
    int _lastValue = 0;

    protected override void Show() {
        GlobalMessenger.Launch(EventMsg.SwitchToUI);
        _chooseBtns = new List<HeroChooseButton>(_toggleGroup.GetComponentsInChildren<HeroChooseButton>());
        foreach (var btn in _chooseBtns)
        {
            btn.Init();
        }

        _startBtn.onClick.AddListener(OnStartClick);
        InputManager.I.LeftStcikEvent += OnSelect;
        InputManager.I.RightBtnEEvent += OnStartGame;
        _index = 0;
        _chooseBtns[_index].SetIsOn(true);
    }

    protected override void Hide() {
        _startBtn.onClick.RemoveListener(OnStartClick);
        InputManager.I.LeftStcikEvent -= OnSelect;
        InputManager.I.RightBtnEEvent -= OnStartGame;
        GlobalMessenger.Launch(EventMsg.SwitchToGameIn);
    }

    void OnStartClick()
    {
        _chooseBtns[_index].InitCharacterData();
        gameObject.SetActive(false);
        GlobalMessenger.Launch(EventMsg.GameStart);
    }

    void OnStartGame(float num, InputManager.ActionState state)
    {
        if (state == InputManager.ActionState.UI)
        {
            if (num == 1)
            {
                _chooseBtns[_index].InitCharacterData();
                gameObject.SetActive(false);
                GlobalMessenger.Launch(EventMsg.GameStart);
            }
        }
    }

    void OnSelect(Vector2 v, InputManager.ActionState state) {
        if (state != InputManager.ActionState.UI || _chooseBtns.Count == 0 )
        {
            return;
        }

        if (_lastValue != (int)v.x)
        {
            if (v.x >= 1)
            {
                _index++;
            }
            else if (v.x <= -1)
            {
                _index--;
            }

            if (_index >= _chooseBtns.Count)
            {
                _index = 0;
            }
            else if (_index < 0)
            {
                _index = _chooseBtns.Count - 1;
            }

            _chooseBtns[_index].SetIsOn(true);
        }
        _lastValue = (int)v.x;
    }
}
