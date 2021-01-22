using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : ActorState
{
    public override void ExecuteEnemy(GameActor actor, float deltaTime){
        Enemy enemy = (Enemy) actor;
        Eexecute0(actor, deltaTime);
    }
    public override void ExecuteTower(GameActor actor, float deltaTime){
        Eexecute0(actor, deltaTime);
    }

    public void Eexecute0(GameActor actor, float deltaTime){
        PositiveActor positiveActor = (PositiveActor) actor;
        if(positiveActor.HaveTarget){
            if(positiveActor.TrackTarget()){
                float dt = Time.deltaTime;
		        positiveActor.ShootProgress+=dt;
                if(positiveActor.ShootProgress>=positiveActor.ShootDuration*1000){
                    positiveActor.Shoot();
                    actor.TurnNextState();
                }
            }else{
                positiveActor.loseTarget();
                actor.TurnNextState();
            }
        }else{
            if(positiveActor.AcquireTarget()){
                positiveActor.StartAttackTime = Time.time;
            }else{
                actor.TurnNextState();
            }
        }
    }

    public override ActorStateType GetActorType()
    {
        return ActorStateType.Attack;
    }
    public override void EnterTower(GameActor actor){
        Enter0(actor);
    }
    public override void EnterEnemy(GameActor actor){
        Enter0(actor);
    }
    public  void Enter0(GameActor actor){
        PositiveActor positiveActor = (PositiveActor) actor;
        positiveActor.LastAttackTime = Time.time;
    }
    protected override bool CanEnter0(GameActor actor)
    {
        if(!(actor is PositiveActor)){
            return false;
        }
        PositiveActor positiveActor = (PositiveActor) actor;
        float now = Time.time;
        return (now-positiveActor.LastAttackTime)>=positiveActor.ShootInterval&&positiveActor.AcquireTarget();
    }
}
