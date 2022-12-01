using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroChooseButton : MonoBehaviour
{
    public string id;
    public Toggle toggle;
    public Sprite selectedSprite;
    public Sprite noSelectedSprite;

    GameObject _characterObject;

    public void Init() {
        toggle.onValueChanged.AddListener(OnClick);
    }

    public void SetIsOn(bool value) {
        toggle.isOn = value;
        toggle.Select();
    }

    public void InitCharacterData() {
        var _character = _characterObject.GetComponent<Character>();
        _character.Init(id);
    }

    void OnClick(bool isOn) {
        if (isOn)
        {
            if (_characterObject == null)
            {
                var playerRoot = GameManager.I.GetPlayerRoot();
                _characterObject = playerRoot.CreateCharacter(id);
            }
        }
        else
        {
            ObjectPool.I.Recycle(_characterObject);
            _characterObject = null;
        }
        toggle.image.sprite = isOn ? selectedSprite : noSelectedSprite;
    }

}
