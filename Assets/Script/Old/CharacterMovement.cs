using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public LayerMask walkable;

    bool isLockOnState = false;
    Vector3 inputShift = Vector3.zero;
    Vector3 smoothInputShift = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector3 lookAtPos = Vector3.zero;
    AnimatorStateInfo curAnimatorInfo;
    QueryTriggerInteraction triggerInteraction;
    float radius = 0.3f;

    PlayerData playerData;
    InputData inputData;
    CameraData cameraData;

    void Start()
    {
        playerData = GameDataManagement.I.PlayerData;
        cameraData = GameDataManagement.I.CameraData;
        inputData = GameDataManagement.I.InputData;

        //EventHandler.Add(EventMsg.ROLL, RollAction);
    }

    void FixedUpdate()
    {
        curAnimatorInfo = playerData.CharacterAnimator.GetCurrentAnimatorStateInfo(0);
        MoveTheCharacter();
    }

    private void MoveTheCharacter()
    {
        if (!curAnimatorInfo.IsName("MoveBlendTree")) {
            return;
        }
        inputShift = new Vector3(inputData.StickLeftValue.x, 0f, inputData.StickLeftValue.y);
        smoothInputShift = Vector3.Lerp(smoothInputShift, inputShift, Time.deltaTime * playerData.MoveSmoothingSpeed);
        playerData.CharacterAnimator.SetFloat("StickValue", smoothInputShift.magnitude);
        moveDirection = CameraDirection(inputShift) * smoothInputShift.magnitude * playerData.MoveSpeed * Time.deltaTime;
        //playerData.CharacterRigidbody.MovePosition(playerData.CharacterModel.position + moveDirection);
        playerData.CharacterModel.position += moveDirection;
        //playerData.CharacterModel.Translate(moveDirection);
        TurnTheCharacter();
    }

    private void TurnTheCharacter()
    {
        if (moveDirection != Vector3.zero) {
            playerData.CharacterModel.rotation = Quaternion.Slerp(playerData.CharacterModel.rotation, 
                                                                  Quaternion.LookRotation(moveDirection), 
                                                                  playerData.TurnSpeed);
        }

        //if (inputMovement.sqrMagnitude > 0.01f) {
        //    Debug.Log(inputMovement.sqrMagnitude);
        //    Quaternion rotation = Quaternion.Slerp(characterRigidbody.rotation,
        //                                          Quaternion.LookRotation(CameraDirection(inputMovement)),
        //                                          turnSpeed);

        //    characterRigidbody.MoveRotation(rotation);
        //}

        //Vector3 rotatePos = this.transform.position + inputMovement;
        //this.transform.LookAt(rotatePos);
    }

    //void RecursivePushback()
    //{

    //    foreach (Collider col in Physics.OverlapSphere(transform.position + transform.up, radius, walkable, triggerInteraction))
    //    {
    //        Vector3 position = transform.position + transform.up;
    //        Vector3 contactPoint;
    //        bool contactPointSuccess = ClosestPointOnSurface(col, position, radius, out contactPoint);

    //        if (!contactPointSuccess)
    //        {
    //            return; 
    //        }


    //        Vector3 v = contactPoint - position;
    //        if (v != Vector3.zero)
    //        {
    //            int layer = col.gameObject.layer;

    //            //col.gameObject.layer = TemporaryLayerIndex;
    //            bool facingNormal = Physics.SphereCast(new Ray(position, v.normalized), TinyTolerance, v.magnitude + TinyTolerance, 1 << TemporaryLayerIndex);

    //            col.gameObject.layer = layer;

    //            if (facingNormal)
    //            {
    //                if (Vector3.Distance(position, contactPoint) < radius)
    //                {
    //                    v = v.normalized * (radius - v.magnitude) * -1;
    //                }
    //                else
    //                {
    //                    continue;
    //                }
    //            }
    //            else
    //            {
    //                v = v.normalized * (radius + v.magnitude);
    //            }

    //            transform.position += v;

    //            //col.gameObject.layer = TemporaryLayerIndex;

    //            // Retrieve the surface normal of the collided point
    //            RaycastHit normalHit;

    //            Physics.SphereCast(new Ray(position + v, contactPoint - (position + v)), TinyTolerance, out normalHit, 1 << TemporaryLayerIndex);

    //            col.gameObject.layer = layer;

    //        }
    //    }
    //}

    private bool ClosestPointOnSurface(Collider collider, Vector3 to, float radius, out Vector3 closestPointOnSurface)
    {
        if (collider is BoxCollider)
        {
            closestPointOnSurface = ClosestPointOnSurface((BoxCollider)collider, to);
            return true;
        }
        else if (collider is SphereCollider)
        {
            //closestPointOnSurface = ClosestPointOnSurface((SphereCollider)collider, to);
            //return true;
        }
        else if (collider is CapsuleCollider)
        {
            //closestPointOnSurface = ClosestPointOnSurface((CapsuleCollider)collider, to);
            //return true;
        }


        Debug.LogError(string.Format("{0} does not have an implementation for ClosestPointOnSurface; GameObject.Name='{1}'", collider.GetType(), collider.gameObject.name));
        closestPointOnSurface = Vector3.zero;
        return false;
    }

    private  Vector3 ClosestPointOnSurface(BoxCollider collider, Vector3 to)
    {
        var ct = collider.transform;
        var local = ct.InverseTransformPoint(to);

        local -= collider.center;
        var halfSize = collider.size * 0.5f;

        var localNorm = new Vector3(
                Mathf.Clamp(local.x, -halfSize.x, halfSize.x),
                Mathf.Clamp(local.y, -halfSize.y, halfSize.y),
                Mathf.Clamp(local.z, -halfSize.z, halfSize.z)
            );

        var dx = Mathf.Min(Mathf.Abs(halfSize.x - localNorm.x), Mathf.Abs(-halfSize.x - localNorm.x));
        var dy = Mathf.Min(Mathf.Abs(halfSize.y - localNorm.y), Mathf.Abs(-halfSize.y - localNorm.y));
        var dz = Mathf.Min(Mathf.Abs(halfSize.z - localNorm.z), Mathf.Abs(-halfSize.z - localNorm.z));

        if (dx < dy && dx < dz)
        {
            localNorm.x = Mathf.Sign(localNorm.x) * halfSize.x;
        }
        else if (dy < dx && dy < dz)
        {
            localNorm.y = Mathf.Sign(localNorm.y) * halfSize.y;
        }
        else if (dz < dx && dz < dy)
        {
            localNorm.z = Mathf.Sign(localNorm.z) * halfSize.z;
        }

        localNorm += collider.center;

        return ct.TransformPoint(localNorm);
    }



    private void RollAction()
    {
        Debug.Log("Roll Action");
        if (curAnimatorInfo.IsName("Standard Roll")) {
            return;
        }
        playerData.CharacterAnimator.applyRootMotion = true;
        playerData.CharacterAnimator.CrossFade("Standard Roll", 0f);
        //playerData.CharacterAnimator.SetBool("IsRoll", inputData.ButtonSouth.isSelect);
        //moveDirection = playerData.CharacterModel.forward * inputShift.z;
        //moveDirection += playerData.CharacterModel.right * inputShift.x;
        //moveDirection.y = 0;
        //playerData.CharacterRigidbody.MovePosition(playerData.CharacterModel.position + moveDirection);
    }

    private void Roll()
    {
        Debug.Log("Roll");
        //if (curAnimatorInfo.IsName("Standard Roll")) {
        //    playerData.CharacterRigidbody.drag = 0;
        //    Vector3 delta = playerData.CharacterAnimator.deltaPosition;
        //    delta.y = 0;
        //    playerData.CharacterRigidbody.velocity = delta / Time.deltaTime;
        //}
    }

    private Vector3 CameraDirection(Vector3 direction)
    {
        Vector3 forward = cameraData.CharacterCamera.transform.forward;
        Vector3 right = cameraData.CharacterCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        return forward * direction.z + right * direction.x;

    }

}
