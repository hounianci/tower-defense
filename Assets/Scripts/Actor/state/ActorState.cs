using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorState
{

    public void Execute(GameActor actor, int deltaTime){
        actor.ExecuteState(this, deltaTime);
    }

    public bool CanEnter(GameActor actor){return CanEnter0(actor);}
    public void Enter(GameActor actor){Enter0(actor);}
    public void Exit(GameActor actor){Exit0(actor);}

    public abstract ActorStateType GetActorType();
    public abstract void ExecuteEnemy(GameActor actor, int deltaTime);
    public abstract void ExecuteTower(GameActor actor, int deltaTime);
    protected virtual void Enter0(GameActor actor){}
    protected virtual bool CanEnter0(GameActor actor){return true;}
    protected virtual void Exit0(GameActor actor){}

}
