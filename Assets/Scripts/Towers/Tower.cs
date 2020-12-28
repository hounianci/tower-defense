using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : GameActor, TargetAble {

	[SerializeField]
	private int id;

	public abstract TowerType TowerType { get; }

	public List<Enemy> blockingEnemy = new List<Enemy>();
	
	private Direction direction = Direction.East;
	protected List<TargetAble> targets;
	public List<TargetAble> Targets{
		get=>targets;
		set{targets=value;}
	}

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
		List<GameTile> currentSkillRange = currentSkill.Tracker.InRangeGameTile(Game.Instance.Board);
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
			skill.Init(int.Parse(infoStr[i]), this);
			skillQueue[i] = skill; 
		}
		direction = Direction.North;
		changeSkill();
		changeShowInRange(true);
		hp = 1000;
	}

	protected void changeSkill(){
		currentSkill = skillQueue[skillQueueIndex%skillQueue.Length];
		skillQueueIndex++;
		currentSkill.Tracker.TurnRange(direction);
		Debug.Log(string.Format("Tower:skill change to {0}",currentSkill.));
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
	protected bool AcquireTarget () {
		targets = currentSkill.Tracker.TrackTarget(TeamId(), 1);
		if (targets.Count>0) {
			lockTarget();
			return true;
		}
		loseTarget();
		targets = null;
		return false;
	}

	//追踪目标
	protected bool TrackTarget () {
		if (targets==null || targets.Count==0) {
			return false;
		}
		List<GameTile> currentSkillRange = currentSkill.Tracker.InRangeGameTile(Game.Instance.Board);
		Vector2Int targetPos = targets[0].GetTilePosition();
		GameTile targetTile = Board.GetTile(targetPos.x, targetPos.y);
		foreach(GameTile tile in currentSkillRange){
			if(tile==targetTile){
				return true;
			}
		}
		//超出范围
		targets = null;
		loseTarget();
		return false;
	}

	public int ApplyDamage(float damage){
		return (int)(hp-damage);
	}

	public Vector3 GetPosition(){
		return transform.position;
	}

	protected virtual void loseTarget(){

	}
	protected virtual void lockTarget(){
		
	}

    public Vector2Int GetTilePosition(){
		return new Vector2Int();
	}
    public int TeamId(){
		return 1;
	}

}