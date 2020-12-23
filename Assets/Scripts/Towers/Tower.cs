using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : GameActor, TargetAble {

	[SerializeField]
	private int id;

	public abstract TowerType TowerType { get; }

	public List<Enemy> blockingEnemy = new List<Enemy>();
	
	private Direction direction = Direction.East;

	Skill currentSkill;
	public Skill CurrentSkill{
		get=>currentSkill;
		set{currentSkill=value;}
	}
	Skill[] skillQueue;
	public Skill[] SkillQueue{
		get=>skillQueue;
		set{skillQueue=value;}
	}
	int skillQueueIndex;
	public int SkillQueueIndex{
		get=>skillQueueIndex;
		set{skillQueueIndex=value;}
	}
	public Direction Direction{
		get => direction;
		set {direction = value;}
	}

	public List<Enemy> BlockingEnemy{
		get => blockingEnemy;
	}

	public int Id{
		get => id;
	}

	public void changeShowInRange(bool show){
		List<GameTile> currentSkillRange = currentSkill.Tracker.InRangeGameTile(this, Game.Instance.Board);
		foreach(GameTile tile in currentSkillRange){
			if(show){
				tile.ShowInRange();
			}else{
				tile.HideInRange();
			}
		}
	}

	public void changeDirection(Direction newDirection){
		changeShowInRange(false);
		currentSkill.Tracker.TurnRange(newDirection);
		changeShowInRange(true);
		this.direction = newDirection;
	}

	protected override void Init0(){
		string towerInfo = FileUtil.readFile(string.Format("Assets/Tower/{0}_Tower.txt", id));
		string[] infoStr = towerInfo.Split(',');
		skillQueue = new Skill[infoStr.Length];
		for(int i=0; i<infoStr.Length; i++){
			Skill skill = new Skill();
			skill.Init(int.Parse(infoStr[i]));
			skillQueue[i] = skill; 
		}
		changeSkill();
	}

	protected void changeSkill(){
		currentSkill = skillQueue[skillQueueIndex%skillQueue.Length];
		skillQueueIndex++;
		currentSkill.Tracker.TurnRange(Direction);
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
		List<GameTile> currentSkillRange = currentSkill.Tracker.InRangeGameTile(this, Game.Instance.Board);
		foreach(GameTile tile in currentSkillRange){
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
		List<GameTile> currentSkillRange = currentSkill.Tracker.InRangeGameTile(this, Game.Instance.Board);
		foreach(GameTile tile in currentSkillRange){
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

    public Vector2Int GetPosition(){
		return new Vector2Int();
	}
    public int TeamId(){
		return 1;
	}

}