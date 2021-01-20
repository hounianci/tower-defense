using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : ActorState
{
    public override void ExecuteEnemy(GameActor actor, float deltaTime){
    }
    public override void ExecuteTower(GameActor actor, float deltaTime){

    }
    public override ActorStateType GetActorType()
    {
        return ActorStateType.Idle;
    }
}
