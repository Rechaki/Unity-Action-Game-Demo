using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitlePanel : UIPanel
{
    [SerializeField]
    Button _enterBtn;
    [SerializeField]
    TextMeshProUGUI _tips;

    protected override void Show() {
        GlobalMessenger.Launch(EventMsg.SwitchToUI);
        _enterBtn.onClick.AddListener(SwitchToGameScene);

        switch (InputManager.I.DevicesType)
        {
            case InputManager.Devices.Keyboard:
                _tips.text = "Press <color=#DBDBDB>L</color> Key";
                break;
            case InputManager.Devices.Gamepad:
                _tips.text = "Press <color=#DBDBDB>Right East</color> Button";
                break;
            default:
                break;
        }

        InputManager.I.RightBtnEEvent += LoadGameScene;
    }

    protected override void Hide() {
        _enterBtn.onClick.RemoveListener(SwitchToGameScene);

        InputManager.I.RightBtnEEvent -= LoadGameScene;
    }

    void SwitchToGameScene() {
        LevelManager.I.LoadScene("Game");
    }

    void LoadGameScene(float num, InputManager.ActionState state) {
        if (state != InputManager.ActionState.UI || num == 0)
        {
            return;
        }

        LevelManager.I.LoadScene("Game");
    }
}
