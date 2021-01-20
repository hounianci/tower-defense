﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PositiveActor : GameActor
{
	protected List<TargetAble> targets;
	public Skill CurrentSkill{get;set;}
	public Skill[] SkillQueue{get;set;}
	public int SkillQueueIndex{get;set;}
	public float ShootProgress{get;set;}
	public float StartAttackTime{get;set;}
	public float LastAttackTime{get;set;}
	public float ShootInterval{get;set;}
	public float ShootDuration{get;set;}
    public bool HaveTarget => targets==null || targets.Count==0 || !targets[0].isAlive();


    protected override void Init0(object[] payloads)
    {		
        base.Init0(payloads);
		ShootInterval = GetInterval();
    }

	protected abstract int GetInterval();

    //寻找目标
    

	//寻找目标
	public bool AcquireTarget () {
        if(CurrentSkill==null){
            return false;
        }
		targets = CurrentSkill.Tracker.TrackTarget(ActorTeamId(), 1);
		if (targets.Count>0) {
			lockTarget();
			return true;
		}
		loseTarget();
		targets = null;
		return false;
	}
	public void ChangeShowInRange(bool show){
		List<GameTile> currentSkillRange = CurrentSkill.Tracker.InRangeGameTile(Game.Instance.Board);
		foreach(GameTile tile in currentSkillRange){
			if(show){
				tile.ShowInRange();
			}else{
				tile.HideInRange();
			}
		}
	}

	public void ChangeSkillDirection(Direction newDirection){
		ChangeShowInRange(false);
		CurrentSkill.Tracker.TurnRange(newDirection);
		ChangeShowInRange(true);
	}

	//追踪目标
	public bool TrackTarget () {
		if (targets==null || targets.Count==0 || !targets[0].isAlive()) {
			return false;
		}
		List<GameTile> currentSkillRange = CurrentSkill.Tracker.InRangeGameTile(Game.Instance.Board);
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

    public override void Recycle0()
    {
        base.Recycle0();
		ChangeShowInRange(false);
    }

    public void Shoot(){
		if(targets==null||targets.Count==0){
			return;
		}
        foreach(TargetAble target in targets){
            if(!target.isAlive()){
                break;
            }
			Debug.Log(string.Format("{0} attack {1}", this, CurrentSkill.Damage));
		    target.ApplyDamage(CurrentSkill.Damage);
        }
        Shoot0();
		ChangeSkill();
    }
	protected void ChangeSkill(){
		CurrentSkill = SkillQueue[SkillQueueIndex%SkillQueue.Length];
		SkillQueueIndex++;
		CurrentSkill.Tracker.TurnRange(SkillDirection());
	}

	protected virtual Direction SkillDirection(){
		return Direction.North;
	}

    protected virtual void Shoot0(){

    }

	public virtual void loseTarget(){

	}
	protected virtual void lockTarget(){
		
	}
	protected override void OnSelected0(){
		ChangeShowInRange(true);
	}
	protected override void OnDisSelected0(){
		ChangeShowInRange(true);
	}
}
