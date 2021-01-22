using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : PositiveActor, TargetAble, BlockerActor {


	public abstract TowerType TowerType { get; }
	private List<Enemy> blockingEnemy = new List<Enemy>();
	public Direction Direction{get;set;}
	public List<Enemy> BlockingEnemy{get;}
	TowerConfigEntry towerConfig;

	public void changeDirection(Direction newDirection){
		base.ChangeSkillDirection(newDirection);
		this.Direction = newDirection;
	}

    protected override int GetInterval()
    {
		return towerConfig.Interval;
    }

    protected override void ExecuteState0(ActorState state, float deltaTime)
    {
		state.ExecuteTower(this, deltaTime);
    }
    public override void EnterState(ActorState state)
    {
		state.EnterTower(this);
    }

    protected override void InitState()
    {
        AllStatus.Add(ActorStateType.Attack, new AttackState());
        AllStatus.Add(ActorStateType.Idle, new IdleState());
        AllStatus.Add(ActorStateType.Outro, new OutroState());

		CurrentState = new IdleState();

		StatusTurns.Add(ActorStateType.Idle, new List<ActorStateType>());
		StatusTurns[ActorStateType.Idle].Add(ActorStateType.Attack);
		StatusTurns.Add(ActorStateType.Attack, new List<ActorStateType>());
		StatusTurns[ActorStateType.Attack].Add(ActorStateType.Idle);
    }
	protected override void Init0(object[] payloads){
		towerConfig = DataManager.GetData<TowerConfigEntry>(typeof(TowerConfig), Id);
		base.Init0(payloads);
		int skillLen = towerConfig.SkillList.Length;
		SkillQueue = new Skill[skillLen];
		for(int i=0; i<skillLen; i++){
			Skill skill = new Skill();
			skill.Init(towerConfig.SkillList[i], this);
			SkillQueue[i] = skill; 
		}
		Direction = Direction.North;
		ChangeSkill();
		ChangeShowInRange(true);
		Hp = towerConfig.Hp;
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

    protected override string DebugName()
    {
		return "[Tower "+identity+"]";
    }
}