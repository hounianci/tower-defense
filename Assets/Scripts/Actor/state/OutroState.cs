using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutroState : ActorState
{
    public override void ExecuteEnemy(GameActor actor, float deltaTime){
        Enemy enemy = (Enemy) actor;
        //死亡结束删除
        if (enemy.Animator.IsDone) {
            enemy.Recycle();
            enemy.NeedRemoveFromUpdate = true;
        }
    }
    public override void ExecuteTower(GameActor actor, float deltaTime){
        Tower tower = (Tower) actor;
        foreach(Enemy enemy in tower.BlockingEnemy){
            enemy.BlockingTower = null;
        }
    }

    public override void EnterEnemy(GameActor actor)
    {
        Enemy enemy = (Enemy) actor;
        enemy.Animator.PlayOutro();
    }

    public override void EnterTower(GameActor actor){
    }

    public override ActorStateType GetActorType()
    {
        return ActorStateType.Outro;
    }
}
