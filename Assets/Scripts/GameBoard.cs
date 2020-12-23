using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour {

	[SerializeField]
	Transform ground = default;

	[SerializeField]
	GameTile tilePrefab = default;

	[SerializeField]
	Texture2D gridTexture = default;

	Vector2Int size;

	GameTile[][] tiles;

	List<GameTile> spawnPoints = new List<GameTile>();

	List<GameActor> updatingContent = new List<GameActor>();

	Queue<GameTile> searchFrontier = new Queue<GameTile>();

	GameTileContentFactory contentFactory;
	TowerFactory towerFactory;

	Tower selectingTower;

	bool showGrid, showPaths;

	public Tower SelectingTower{
		get=>selectingTower;
		set{
			if(selectingTower!=null){
				selectingTower.changeShowInRange(false);
			}
			selectingTower=value;
			selectingTower.changeShowInRange(true);
		}
	}

	public bool ShowGrid {
		get => showGrid;
		set {
			showGrid = value;
			Material m = ground.GetComponent<MeshRenderer>().material;
			if (showGrid) {
				m.mainTexture = gridTexture;
				m.SetTextureScale("_MainTex", size);
			}
			else {
				m.mainTexture = null;
			}
		}
	}

	public bool ShowPaths {
		get => showPaths;
		set {
			showPaths = value;
			if (showPaths) {
				foreach (GameTile[] row in tiles) {
					foreach(GameTile tile in row){
						tile.ShowPath();
					}
				}
			}
			else {
				foreach (GameTile[] row in tiles) {
					foreach(GameTile tile in row){
						tile.HidePath();
					}
				}
			}
		}
	}

	public int SpawnPointCount => spawnPoints.Count;

	public void Initialize (
		Vector2Int size, GameTileContentFactory contentFactory
	) {
		this.size = size;
		this.contentFactory = contentFactory;
		ground.localScale = new Vector3(size.x, size.y, 1f);

		Vector2 offset = new Vector2(
			(size.x - 1) * 0.5f, (size.y - 1) * 0.5f
		);
		tiles = new GameTile[size.y][];
		for (int y = 0; y < size.y; y++) {
			tiles[y] = new GameTile[size.x];
			for (int x = 0; x < size.x; x++) {
				GameTile tile = tiles[y][x] = Instantiate(tilePrefab);
				tile.X = x;
				tile.Y = y;
				tile.transform.SetParent(transform, false);
				tile.transform.localPosition = new Vector3(
					x - offset.x, 0f, y - offset.y
				);

				if (x > 0) {
					GameTile.MakeEastWestNeighbors(tile, tiles[y][x-1]);
				}
				if (y > 0) {
					GameTile.MakeNorthSouthNeighbors(tile, tiles[y-1][x]);
				}

				tile.IsAlternative = (x & 1) == 0;
				if ((y & 1) == 0) {
					tile.IsAlternative = !tile.IsAlternative;
				}
			}
		}
		Clear();
	}

	public List<GameTile> GenericDestinationPath(GameTile from, GameTile to){
		int[][] map = new int[size.y][];
		bool[][] visited = new bool[size.y][];
		int[][][] pathInfo = new int[size.y][][];
		for(int i=0; i<size.y; i++){
			map[i] = new int[size.x];
			visited[i] = new bool[size.x];
			pathInfo[i] = new int[size.x][];
			for(int j=0; j<size.x; j++){
				map[i][j]=-1;
				visited[i][j] = false;
				pathInfo[i][j] = new int[2];
			}
		}
		GameTile start = from;
		Queue<GameTile> queue = new Queue<GameTile>();
		queue.Enqueue(start);
		visited[start.Y][start.X] = true;
		int depth = 0;
		while(queue.Count>0){
			int count = queue.Count;
			for(int i=0; i<count; i++){
				GameTile tile = queue.Dequeue();
				if(tile==to){
					map[tile.Y][tile.X] = depth;
					queue.Clear();
					break;
				}
				AddTileToQueue(tile, tile.West, map, visited, pathInfo, depth+1, queue);
				AddTileToQueue(tile, tile.East, map, visited, pathInfo, depth+1, queue);
				AddTileToQueue(tile, tile.North, map, visited, pathInfo, depth+1, queue);
				AddTileToQueue(tile, tile.South, map, visited, pathInfo, depth+1, queue);
			}
			depth++;
		}
		List<GameTile> path = new List<GameTile>();
		GameTile end = to;
		path.Add(to);
		if(map[end.Y][end.X]==-1){
			return null;
		}
		while(end != from){
			GameTile tmp = GetGameTile(pathInfo[end.Y][end.X][1], pathInfo[end.Y][end.X][0]);
			path.Add(tmp);
			end = tmp;
		}
		path.RemoveAt(path.Count-1);
		path.Reverse();
		return path;
	}

	private void AddTileToQueue(GameTile baseTile, GameTile neighbor, int[][] map, bool[][] visited, int[][][] pathInfo, int depth, Queue<GameTile> queue){
		if(neighbor==null){
			return;
		}
		if(!CanPass(neighbor)){
			return;
		}
		if(visited[neighbor.Y][neighbor.X]){
			return;
		}
		visited[neighbor.Y][neighbor.X] = true;
		map[neighbor.Y][neighbor.X] = depth;
		pathInfo[neighbor.Y][neighbor.X] = new int[]{baseTile.Y, baseTile.X};
		queue.Enqueue(neighbor);
	}

	private bool CanPass(GameTile tile){
		if(tile.Content.Type == GameTileContentType.Wall){
			return false;
		}
		return true;
	}

	public GameTile GetGameTile(int x, int y){
		return tiles[y][x];
	}

	public void Clear () {
		foreach (GameTile[] row in tiles) {
			foreach(GameTile tile in row){
				tile.Content = contentFactory.Get(GameTileContentType.Empty);
			}
		}
		spawnPoints.Clear();
		updatingContent.Clear();
	}

	public void GameUpdate () {
		for (int i = 0; i < updatingContent.Count; i++) {
			updatingContent[i].GameUpdate();
		}
	}

	public void ToggleDestination (GameTile tile) {
		if (tile.Content.Type == GameTileContentType.Destination) {
			tile.Content = contentFactory.Get(GameTileContentType.Empty);
			if (!FindPaths()) {
				tile.Content =
					contentFactory.Get(GameTileContentType.Destination);
				FindPaths();
			}
		}
		else if (tile.Content.Type == GameTileContentType.Empty) {
			tile.Content = contentFactory.Get(GameTileContentType.Destination);
			FindPaths();
		}
	}

	public void ToggleWall (GameTile tile) {
		if (tile.Content.Type == GameTileContentType.Wall) {
			tile.Content = contentFactory.Get(GameTileContentType.Empty);
			FindPaths();
		}
		else if (tile.Content.Type == GameTileContentType.Empty) {
			tile.Content = contentFactory.Get(GameTileContentType.Wall);
			if (!FindPaths()) {
				tile.Content = contentFactory.Get(GameTileContentType.Empty);
				FindPaths();
			}
		}
	}

	public void ToggleSpawnPoint (GameTile tile) {
		if (tile.Content.Type == GameTileContentType.SpawnPoint) {
			if (spawnPoints.Count > 1) {
				spawnPoints.Remove(tile);
				tile.Content = contentFactory.Get(GameTileContentType.Empty);
			}
		}
		else if (tile.Content.Type == GameTileContentType.Empty) {
			tile.Content = contentFactory.Get(GameTileContentType.SpawnPoint);
			spawnPoints.Add(tile);
		}
	}

	public void ToggleTower (GameTile tile, TowerType towerType) {
		if(tile.Content.OnboardTargets.ContainsKey(1)){
			return;
		}
		Tower tower = towerFactory.Get(towerType, tile);
		List<TargetAble> towers = new List<TargetAble>();
		towers.Add(tower);
		tile.Content.OnboardTargets.Add(1, towers);
		selectingTower = tower;
		List<Enemy> enemies = tile.Content.Enemies;
		foreach(Enemy enemy in tile.Content.Enemies){
			tower.enemyPass(enemy);
		}
		updatingContent.Add(tower);
	}

	public void selectingTowerChangeDirection(Direction direction){
		if(selectingTower!=null){
			selectingTower.changeDirection(direction);
		}
	}

	public List<GameTile> targetTailes(Vector2Int trackerPos, Vector2Int rangeOffset, List<List<int>> range){
		List<GameTile> results = new List<GameTile>();
		int startX = Mathf.Max(0, trackerPos.x-rangeOffset.x);
		int startY = Mathf.Max(0, trackerPos.y-rangeOffset.y);
		if(range==null){
			for(int i=0; i<tiles.Length; i++){
				results.AddRange(tiles[i]);
			}
		}else{
			for(int i=0; i<range.Count&&startX+i<size.x; i++){
				for(int j=0; j<range[i].Count&&startY+j<size.y; j++){
					if(range[i][j]==1){
						results.Add(tiles[startY+j][startX+i]);
					}
				}
			}
		}
		return results;
	}

	public GameTile GetSpawnPoint (int index) {
		return spawnPoints[index];
	}

	public GameTile GetTile (Ray ray) {
		if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1)) {
			int x = (int)(hit.point.x + size.x * 0.5f);
			int y = (int)(hit.point.z + size.y * 0.5f);
			if (x >= 0 && x < size.x && y >= 0 && y < size.y) {
				return tiles[y][x];
			}
		}
		return null;
    }

    public GameTile GetTile(int x, int y)
    {
        return tiles[y][x];
    }

	public List<GameTile> FindEnemyPath(int[] start, List<int[]> mainTile){
		List<GameTile> path = new List<GameTile>();
		GameTile startTile = GetGameTile(start[1], start[0]);
		for(int i=0; i<mainTile.Count; i++){
			GameTile endTile = GetGameTile(mainTile[i][1], mainTile[i][0]);
			path.AddRange(GenericDestinationPath(startTile, endTile));
			startTile = endTile;
		}
		return path;
	}

	public bool FindPaths(){
		return true;
	}

}