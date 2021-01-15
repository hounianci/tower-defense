using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : ActorState
{
    public override void ExecuteEnemy(GameActor actor, int deltaTime){

    }
    public override void ExecuteTower(GameActor actor, int deltaTime){

    }
    public override ActorStateType GetActorType()
    {
        return ActorStateType.Move;
    }
}
