using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : GameTileContent {

	[SerializeField]
	private int id;

	private List<List<int>> targetRange;
	private Vector2Int targetRangeSelfPos;

	public abstract TowerType TowerType { get; }

	public List<Enemy> blockingEnemy = new List<Enemy>();
	
	private Direction direction = Direction.East;

	private List<GameTile> rangeGameTiles;

	public Direction Direction{
		get => direction;
		set {direction = value;}
	}
	public List<GameTile> RangeGameTiles{
		get=>rangeGameTiles;
		set{rangeGameTiles=value;}
	}

	public Vector2Int TargetRangeSelfPos{
		get => targetRangeSelfPos;
		set{targetRangeSelfPos = value;}
	}

	public List<Enemy> BlockingEnemy{
		get => blockingEnemy;
	}

	public int Id{
		get => id;
	}

	public void changeShowInRange(bool show){
		foreach(GameTile tile in rangeGameTiles){
			if(show){
				tile.ShowInRange();
			}else{
				tile.HideInRange();
			}
		}
	}

	public void changeDirection(Direction newDirection){
		DirectionChange change = direction.GetDirectionChangeTo(newDirection);
		foreach(GameTile tile in rangeGameTiles){
			changeShowInRange(false);
		}
		if(change==DirectionChange.TurnLeft){
			targetRange = FileUtil.matrixTurnLeft(targetRange);
		}else if(change==DirectionChange.TurnRight){
			targetRange = FileUtil.matrixTurnRight(targetRange);
		}else if(change==DirectionChange.TurnAround){
			if(newDirection==Direction.North || newDirection==Direction.South){
				targetRange = FileUtil.matrixTurnAround2(targetRange);
			}else{
				targetRange = FileUtil.matrixTurnAround(targetRange);
			}
		}
		loadSelfRangePos();
		rangeGameTiles = Game.Instance.Board.targetTailes(X, Y, targetRangeSelfPos.x, targetRangeSelfPos.y, 1, targetRange);
		changeShowInRange(true);
		this.direction = newDirection;
	}

	protected override void init0(){
		loadTargetRange();
		rangeGameTiles = Game.Instance.Board.targetTailes(X, Y, targetRangeSelfPos.x, targetRangeSelfPos.y, 1, targetRange);
		changeShowInRange(true);
		
	}

	public void loadTargetRange(){
		targetRange = FileUtil.readFileMatrix(string.Format("Assets/TargetRange/{0}.txt", id));
		loadSelfRangePos();
		Debug.Log(targetRangeSelfPos);
	}

	public void loadSelfRangePos(){
		for(int i=0; i<targetRange.Count; i++){
			for(int j=0; j<targetRange[i].Count; j++){
				if(targetRange[i][j]==2){
					targetRangeSelfPos = new Vector2Int(i, j);
				}
			}
		}
	}

	protected virtual int blockNum(){
		return 0;
	}

	public bool enemyPass(Enemy enemy){
		if(blockNum()>blockingEnemy.Count){
			blockingEnemy.Add(enemy);
			enemy.BlockingTower = this;
			return true;
		}else{
			return false;
		}
	}

	//寻找目标
	protected bool AcquireTarget (out TargetPoint target) {
		List<TargetPoint> list = new List<TargetPoint>();
		foreach(GameTile tile in rangeGameTiles){
			if(tile.Content.Enemies!=null && tile.Content.Enemies.Count>0){
				foreach(Enemy enemy in tile.Content.Enemies){
					list.Add(enemy.TargetPoint);
				}
			}
		}
		if (list.Count>0) {
			target = list[Random.Range(0, list.Count)];
			lockTarget();
			return true;
		}
		loseTarget();
		target = null;
		return false;
	}

	//追踪目标
	protected bool TrackTarget (ref TargetPoint target) {
		if (target == null || !target.Enemy.IsValidTarget) {
			return false;
		}
		foreach(GameTile tile in rangeGameTiles){
			if(tile==target.Enemy.TileFrom){
				return true;
			}
		}
		//超出范围
		target = null;
		loseTarget();
		return false;
	}

	protected virtual void loseTarget(){

	}
	protected virtual void lockTarget(){
		
	}

}