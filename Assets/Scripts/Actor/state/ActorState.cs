using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorState
{

    public void Execute(GameActor actor, float deltaTime){
        actor.ExecuteState(this, deltaTime);
    }

    public bool CanEnter(GameActor actor){return CanEnter0(actor);}
    public void Enter(GameActor actor){actor.EnterState(this);}
    public void Exit(GameActor actor){Exit0(actor);}

    public abstract ActorStateType GetActorType();
    public abstract void ExecuteEnemy(GameActor actor, float deltaTime);
    public abstract void ExecuteTower(GameActor actor, float deltaTime);
    public virtual void EnterEnemy(GameActor actor){}
    public virtual void EnterTower(GameActor actor){}
    protected virtual bool CanEnter0(GameActor actor){return true;}
    protected virtual void Exit0(GameActor actor){}

}
