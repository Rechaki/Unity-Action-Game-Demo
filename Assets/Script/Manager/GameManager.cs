using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public bool isGameOver { get; private set; }
	public bool Paused { get; private set; }

	InputManager inputMgr;
	bool _inited = false;

	void Awake() {
		if (!_inited)
		{
			Init();
		}
	}

    void Update()
	{
		inputMgr.Update();

	}

    public void Init() {
		Application.targetFrameRate = 30;

		inputMgr = new InputManager();
		inputMgr.Init();
		//https://www.twblogs.net/a/5b83b2d02b71777a2efcf61d
		//DataManager.I.Init();
		isGameOver = false;
		_inited = true;

		//DontDestroyOnLoad(this);
	}

	private void GameClear() {
		//UIManager.I.Open(AssetPath.GAME_CLEAR_PANEL);
	}

	private void GameOver() {
		isGameOver = true;
        //UIManager.I.Open(AssetPath.GAME_OVER_PANEL);
	}

	private void Restart() {
		isGameOver = false;
	}

	public void SaveData() {
		//DataManager.SaveData();
	}

	public void DeleteSaveData() {
		//DataManager.DeleteSaveData();
	}

}
