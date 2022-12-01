using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public float maxSizeOffset = 2f;
	public float createSlimeSpeed = 5f;
	public float createArcherSpeed = 8f;
	public int killedEnemyNumToNextEnemy = 20;
	public int KilledEnemyNumToBoss = 30;
	public int stage = 0;
	public Transform[] bossMovePoints;

	public bool isGameOver { get; private set; }
	public bool Paused { get; private set; }
	public Vector2 ScreenSize => _maxPos;

	enum CreateEnemyState
    {
		None,
		Slime,
		Archer,
		Boss
    }

	CreateEnemyState _createState = CreateEnemyState.None;
	Player _playerRoot;
	Vector2 _maxPos;
	bool _inited = false;
	bool _isBossCreated = false;
	float _createSlimeTimer = 0;
	float _createArcherTimer = 0;
	int _killedNum = 0;
	string[] _enemySlimeIDs = { "E0000", "E0001", "E0002", "E0003", "E0004", "E0005" };
	string[] _enemyArcherIDs = { "E0011", "E0012" };
	string[] _enemyBoosIDs = { "E0010" };

	void Awake() {
		if (!_inited)
		{
			Init();
		}
	}

	void OnEnable() {
		if (!_inited)
		{
			Init();
		}
	}

    void Update()
	{
		if (_killedNum > killedEnemyNumToNextEnemy)
		{
			_createState = CreateEnemyState.Archer;
        }
        if (_killedNum > KilledEnemyNumToBoss)
        {
			_createState = CreateEnemyState.Boss;
        }

		switch (_createState)
        {
            case CreateEnemyState.None:
                break;
            case CreateEnemyState.Slime:
				CreateSlime();
                break;
            case CreateEnemyState.Archer:
				CreateSlime();
				CreateArcher();
				break;
            case CreateEnemyState.Boss:
				if (!_isBossCreated)
				{
					GlobalMessenger.Launch(EventMsg.BossComing);
					_isBossCreated = true;
				}
				break;
            default:
                break;
        }

		_createSlimeTimer += Time.deltaTime;
		_createArcherTimer += Time.deltaTime;

	}

    public void Init() {
		Application.targetFrameRate = 60;
		DataManager.I.Init();
		SetMaxPosition();
		GlobalMessenger.AddListener(EventMsg.GameStart, () => { _createState = CreateEnemyState.Slime; });
		GlobalMessenger.AddListener(EventMsg.KilledTheEnemy, () => { _killedNum++; });
		GlobalMessenger.AddListener(EventMsg.BossBattleStart, CreateBoss);
		isGameOver = false;
		_createSlimeTimer = 0;
		_createArcherTimer = 0;
		_killedNum = 0;
		_isBossCreated = false;
		_inited = true;
		_createState = CreateEnemyState.None;

		//DontDestroyOnLoad(this);
	}

	public Player GetPlayerRoot() {
		_playerRoot = UnityEngine.Object.FindObjectOfType<Player>();
		if (_playerRoot == null)
		{
			var obj = new GameObject(typeof(Player).Name);
			obj.transform.position = Vector3.zero;
			_playerRoot = obj.AddComponent<Player>();
		}
		return _playerRoot;
	}

	void SetMaxPosition() {
		Vector2 v = new Vector2(Screen.width, Screen.height);
		var pos = Camera.main.ScreenToWorldPoint(v);
		_maxPos = new Vector2(pos.x - maxSizeOffset, pos.y - maxSizeOffset);
	}

	void CreateSlime()
	{
		if (_createSlimeTimer > createSlimeSpeed)
		{
			CreateEnemy(_enemySlimeIDs);
			_createSlimeTimer = 0f;

		}

	}

	void CreateArcher()
	{
		if (_createArcherTimer > createArcherSpeed)
		{
			CreateEnemy(_enemyArcherIDs);
			_createArcherTimer = 0f;
		}

	}

	void CreateBoss()
    {
		var enemyPefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_PREFAB + _enemyBoosIDs[stage]);
		var enemyObject = ObjectPool.I.Create(enemyPefab);
		enemyObject.transform.position = Vector3.zero;
		enemyObject.GetComponent<EnemyUnit>().InitData();
		enemyObject.SetActive(true);
	}

	void CreateEnemy(string[] ids) {
		int index = UnityEngine.Random.Range(0, ids.Length);
		var enemyPefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_PREFAB + ids[index]);
		var enemyObject = ObjectPool.I.Create(enemyPefab);
		var x = UnityEngine.Random.Range(-_maxPos.x, _maxPos.x);
		var y = UnityEngine.Random.Range(-_maxPos.y, _maxPos.y);
		enemyObject.transform.position = new Vector3(x, y, 0);
		enemyObject.GetComponent<EnemyUnit>().InitData();
		enemyObject.SetActive(true);
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
