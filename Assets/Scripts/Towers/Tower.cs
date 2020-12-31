using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : PositiveActor, TargetAble, BlockerActor {


	public abstract TowerType TowerType { get; }
	public List<Enemy> blockingEnemy = new List<Enemy>();
	public Direction Direction{get;set;}
	public List<Enemy> BlockingEnemy{get;}


	public void changeDirection(Direction newDirection){
		base.ChangeSkillDirection(newDirection);
		this.Direction = newDirection;
	}

	protected override void Init0(){
		base.Init0();
		string towerInfo = FileUtil.readFile(string.Format("Assets/Tower/{0}_Tower.txt", Id));
		string[] infoStr = towerInfo.Split(',');
		SkillQueue = new Skill[infoStr.Length];
		for(int i=0; i<infoStr.Length; i++){
			Skill skill = new Skill();
			skill.Init(int.Parse(infoStr[i]), this);
			SkillQueue[i] = skill; 
		}
		Direction = Direction.North;
		ChangeSkill();
		ChangeShowInRange(true);
		Hp = 10;
	}

	public override bool Update0 () {
		if(Hp<=0){
			foreach(Enemy enemy in blockingEnemy){
				enemy.BlockingTower = null;
			}
			Recycle();
			return false;
		}else{
			return base.Update0();
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

	public bool IsBlockByMe(GameActor actor){
		foreach(GameActor enemy in blockingEnemy){
			if(actor == enemy){
				return true;
			}
		}
		return false;
	}
	public int ApplyDamage(float damage){
        Hp -= (int)damage;
		return Hp;
	}

	protected override Direction SkillDirection(){
		return Direction;
	}
    public override int ActorTeamId()
    {
		return TeamId();
    }

    public int TeamId(){
		return 1;
	}
}