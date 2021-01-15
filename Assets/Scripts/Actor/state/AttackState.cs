using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : ActorState
{
    public override void ExecuteEnemy(GameActor actor, int deltaTime){
        Eexecute0(actor, deltaTime);
    }
    public override void ExecuteTower(GameActor actor, int deltaTime){
        Eexecute0(actor, deltaTime);
    }

    public void Eexecute0(GameActor actor, int deltaTime){
        PositiveActor positiveActor = (PositiveActor) actor;
        if(positiveActor.HaveTarget){
            if(positiveActor.TrackTarget()){
		        positiveActor.ShootProgress+=Time.deltaTime;
                if(positiveActor.ShootProgress>=positiveActor.ShootDuration){
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
    protected override void Enter0(GameActor actor)
    {
    }
    protected override bool CanEnter0(GameActor actor)
    {
        if(!(actor is PositiveActor)){
            return false;
        }
        PositiveActor positiveActor = (PositiveActor) actor;
        return (Time.time-positiveActor.LastAttackTime)>=positiveActor.ShootInterval;
    }
}
