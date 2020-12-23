using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	const float pausedTimeScale = 0f;
	[SerializeField]
	int mapId;

	[SerializeField]
	Vector2Int boardSize = new Vector2Int(11, 11);

	[SerializeField]
	GameBoard board = default;

	public GameBoard Board{
		get => board;
	}

	[SerializeField]
	GameTileContentFactory tileContentFactory = default;

	[SerializeField]
	TowerFactory towerFactory = default;

	[SerializeField]
	WarFactory warFactory = default;

	[SerializeField]
	GameScenario scenario = default;

	[SerializeField, Range(0, 100)]
	int startingPlayerHealth = 10;

	[SerializeField, Range(1f, 10f)]
	float playSpeed = 1f;

	int playerHealth;

    ArrayList initWall = new ArrayList();
    ArrayList initSpwan = new ArrayList();
    ArrayList initDestination = new ArrayList();
    
    GameScenario.State activeScenario;

	TowerType selectedTowerType;

	GameBehaviorCollection enemies = new GameBehaviorCollection();
	GameBehaviorCollection nonEnemies = new GameBehaviorCollection();

	Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

	static Game instance;

	public static Game Instance{
		get => instance;
	}

	public static void EnemyReachedDestination () {
		instance.playerHealth -= 1;
	}

	public static void SpawnEnemy (EnemyFactory factory, EnemyType type) {
		GameTile spawnPoint = instance.board.GetSpawnPoint(
            UnityEngine.Random.Range(0, instance.board.SpawnPointCount)
		);
		Enemy enemy = factory.Get(type);
		enemy.Board = instance.Board;
		enemy.SpawnOn(spawnPoint);
		enemy.InitPath(instance.mapId);
		instance.enemies.Add(enemy);
		enemy.PrepareIntro();
	}

	public static Explosion SpawnExplosion () {
		Explosion explosion = instance.warFactory.Explosion;
		instance.nonEnemies.Add(explosion);
		return explosion;
	}

	public static Shell SpawnShell () {
		Shell shell = instance.warFactory.Shell;
		instance.nonEnemies.Add(shell);
		return shell;
	}

	void OnEnable () {
		instance = this;
	}

	void Awake ()
    {
        InitGame();
    }

    void InitGame()
    {
        ReadFile();
        playerHealth = startingPlayerHealth;
        board.Initialize(boardSize, tileContentFactory);
        foreach (Vector2Int v in initSpwan)
        {
            GameTile tile = board.GetTile(v.x, v.y);
            board.ToggleSpawnPoint(tile);
        }
        foreach (Vector2Int v in initDestination)
        {
            GameTile tile = board.GetTile(v.x, v.y);
            board.ToggleDestination(tile);
        }
        foreach (Vector2Int v in initWall)
        {
            GameTile tile = board.GetTile(v.x, v.y);
            board.ToggleWall(tile);
        }
        board.ShowGrid = true;
        activeScenario = scenario.Begin();
    }

	void BeginNewGame ()
    {
        board.Clear();
        InitGame();
    }

	void OnValidate () {
		if (boardSize.x < 2) {
			boardSize.x = 2;
		}
		if (boardSize.y < 2) {
			boardSize.y = 2;
		}
	}

	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			HandleTouch();
		}
		else if (Input.GetMouseButtonDown(1)) {
			HandleAlternativeTouch();
		}

		if (Input.GetKeyDown(KeyCode.V)) {
			board.ShowPaths = !board.ShowPaths;
		}
		if (Input.GetKeyDown(KeyCode.G)) {
			board.ShowGrid = !board.ShowGrid;
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			board.selectingTowerChangeDirection(Direction.North);
		}
		if (Input.GetKeyDown(KeyCode.A)) {
			board.selectingTowerChangeDirection(Direction.West);
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			board.selectingTowerChangeDirection(Direction.South);
		}
		if (Input.GetKeyDown(KeyCode.D)) {
			board.selectingTowerChangeDirection(Direction.East);
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			selectedTowerType = TowerType.Laser;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			selectedTowerType = TowerType.Mortar;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			selectedTowerType = TowerType.Warrior;
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			Time.timeScale =
				Time.timeScale > pausedTimeScale ? pausedTimeScale : playSpeed;
		}
		else if (Time.timeScale > pausedTimeScale) {
			Time.timeScale = playSpeed;
		}

		if (Input.GetKeyDown(KeyCode.B)) {
			BeginNewGame();
		}

		if (playerHealth <= 0 && startingPlayerHealth > 0) {
			Debug.Log("Defeat!");
			BeginNewGame();
		}

		if (!activeScenario.Progress() && enemies.IsEmpty) {
			Debug.Log("Victory!");
			BeginNewGame();
			activeScenario.Progress();
		}

		enemies.GameUpdate();
		Physics.SyncTransforms();
		board.GameUpdate();
		nonEnemies.GameUpdate();
	}

	void HandleAlternativeTouch () {
		GameTile tile = board.GetTile(TouchRay);
		if (tile != null) {
			if (Input.GetKey(KeyCode.LeftShift)) {
				board.ToggleDestination(tile);
			}
			else {
				board.ToggleSpawnPoint(tile);
			}
		}
	}

	void HandleTouch () {
		GameTile tile = board.GetTile(TouchRay);
		if (tile != null) {
			if (Input.GetKey(KeyCode.LeftShift)) {
				board.ToggleTower(tile, selectedTowerType);
			}else if(Input.GetKey(KeyCode.LeftAlt)){
				if(tile.Content.OnboardTargets.ContainsKey(1)){
					board.SelectingTower = (Tower)tile.Content.OnboardTargets[1][0];
				}
			}
			else {
				board.ToggleWall(tile);
			}
		}
	}

    void ReadFile()
    {
		List<List<int>> matrix = FileUtil.readFileMatrix(string.Format("Assets/Map/{0}.txt", mapId));
	 	for(int row=0; row<matrix.Count;row++){	
			List<int> rowContent = matrix[row];
			for(int col=0; col<rowContent.Count; col++){
				int colContent = rowContent[col];
				switch (colContent){
					case 1:
						initWall.Add(new Vector2Int(col, row));
					break;
					case 2:
                        initSpwan.Add(new Vector2Int(col, row));
					break;
					case 3:
                        initDestination.Add(new Vector2Int(col, row));
					break;
				}
			}
		}
        boardSize = new Vector2Int(matrix.Count, matrix[0].Count);
    }

}