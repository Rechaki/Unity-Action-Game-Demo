using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManagement : Singleton<GameDataManagement>
{
    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    private InputData inputData;
    [SerializeField]
    private CameraData cameraData;

    public PlayerData PlayerData => playerData;
    public InputData InputData => inputData;
    public CameraData CameraData => cameraData;

}
