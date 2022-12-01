using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    //public Character CurrentCharacter => _character;

    //Character _character;
    //GameObject characterObject;

    void Start() {
        

        //EventMessenger.AddListener(EventMsg.GameOver, Dead);
    }

    public GameObject CreateCharacter(string id) {
        var characterPefab = ResourceManager.I.Load<GameObject>(AssetPath.CHARACTER_PREFAB + id);
        var characterObject = ObjectPool.I.Create(characterPefab);
        characterObject.transform.parent = transform;
        characterObject.transform.position = Vector3.zero;
        characterObject.SetActive(true);
        return characterObject;
    }

}
