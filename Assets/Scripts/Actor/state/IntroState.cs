using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroState : ActorState
{
    public override void ExecuteEnemy(GameActor actor, float deltaTime){
        Enemy enemy = (Enemy) actor;
        if(enemy.Animator.IsDone){
            enemy.TurnNextState();
        }
    }

    public override void EnterEnemy(GameActor actor)
    {
        Enemy enemy = (Enemy) actor;
		enemy.Animator.PlayIntro();
    }
    public override void ExecuteTower(GameActor actor, float deltaTime){

    }
    public override ActorStateType GetActorType()
    {
        return ActorStateType.Intro;
    }
}
