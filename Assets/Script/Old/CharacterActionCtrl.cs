//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CharacterActionCtrl : MonoBehaviour
//{
//    Vector3 moveDirection = Vector3.zero;

//    PlayerData playerData;
//    InputData inputData;
//    CameraData cameraData;

//    void Start()
//    {
//        playerData = GameDataManagement.Instance.PlayerData;
//        cameraData = GameDataManagement.Instance.CameraData;
//        inputData = GameDataManagement.Instance.InputData;
//        EventHandler.Add(EventMsg.ROLL, RollAction);
//    }

//    private void RollAction()
//    {
//        Debug.Log("Roll Action");
//        if (inputData.StickLeftValue == Vector2.zero) {
//            moveDirection = playerData.CharacterModel.forward　* playerData.MoveSpeed;
//        } else {
//            moveDirection = RollDirection(inputData.StickLeftValue) * playerData.MoveSpeed;
//        }
//        playerData.CharacterAnimator.CrossFade("Standard Roll", 0f);
//        playerData.CharacterAnimator.SetBool("IsRoll", inputData.ButtonSouth.isSelect);
//        //playerData.CharacterRigidbody.MovePosition(playerData.CharacterModel.position + moveDirection);
//    }

//    private Vector3 RollDirection(Vector3 direction)
//    {
//        Vector3 forward = playerData.CharacterModel.forward;
//        Vector3 right = playerData.CharacterModel.right;

//        forward.y = 0f;
//        right.y = 0f;

//        return forward * direction.z + right * direction.x;

//    }
//}
