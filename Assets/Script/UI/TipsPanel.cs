using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsPanel : UIPanel
{
    [SerializeField]
    Animator _animator;
    [SerializeField]
    GameObject _gamePadTips;
    [SerializeField]
    GameObject _keyBoardTips;
    [SerializeField]
    GameObject _gameOver;
    [SerializeField]
    GameObject _gameClear;
    [SerializeField]
    Button _gameOverBtn;
    [SerializeField]
    Button _gameClearBtn;

    protected override void Show()
    {
        ShowTips();
        _gameOverBtn.onClick.AddListener(OnOverClick);
        _gameClearBtn.onClick.AddListener(OnClearClick);

        InputManager.I.RightBtnEEvent += OnNext;

        GlobalMessenger.AddListener(EventMsg.GameStart, HideTips);
        GlobalMessenger.AddListener(EventMsg.BossComing, PlayBossIn);
        GlobalMessenger.AddListener(EventMsg.GameClear, ShowGameClear);
        GlobalMessenger.AddListener(EventMsg.GameOver, ShowGameOver);
    }

    protected override void Hide()
    {
        HideTips();
        _gameOverBtn.onClick.RemoveListener(OnOverClick);
        _gameClearBtn.onClick.RemoveListener(OnClearClick);

        InputManager.I.RightBtnEEvent -= OnNext;

        GlobalMessenger.RemoveListener(EventMsg.GameStart, HideTips);
        GlobalMessenger.RemoveListener(EventMsg.BossComing, PlayBossIn);
        GlobalMessenger.RemoveListener(EventMsg.GameClear, ShowGameClear);
        GlobalMessenger.RemoveListener(EventMsg.GameOver, ShowGameOver);
    }

    void PlayBossIn()
    {
        _animator.SetTrigger("BossIn");
        GlobalMessenger.Launch(EventMsg.BossBattleStart);
    }

    void ShowTips() {
        _gamePadTips.SetActive(InputManager.I.DevicesType == InputManager.Devices.Gamepad);
        _keyBoardTips.SetActive(InputManager.I.DevicesType == InputManager.Devices.Keyboard);
    }

    void HideTips() {
        _gamePadTips.SetActive(false);
        _keyBoardTips.SetActive(false);
    }

    void OnNext(float num, InputManager.ActionState state) {
        if (state != InputManager.ActionState.UI || num == 0)
        {
            return;
        }

        if (_gameClear.activeSelf)
        {
            OnClearClick();
        }
        else if(_gameOver.activeSelf)
        {
            OnOverClick();
        }
    }

    void ShowGameClear() {
        GlobalMessenger.Launch(EventMsg.SwitchToUI);
        _gameClear.SetActive(true);
    }
    void ShowGameOver() {
        GlobalMessenger.Launch(EventMsg.SwitchToUI);
        _gameOver.SetActive(true);
    }

    void OnClearClick() {
        LevelManager.I.LoadScene("Ending");
    }

    void OnOverClick() {
        LevelManager.I.LoadScene("Ending");
    }

}
