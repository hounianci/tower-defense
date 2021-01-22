using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : ActorState
{
    public override void ExecuteEnemy(GameActor actor, float deltaTime){
        Enemy enemy = (Enemy) actor;
		if (enemy.TileTo == null) {
			Game.EnemyReachedDestination();
			enemy.TurnState(ActorStateType.Outro);
			enemy.TargetPointCollider.enabled = false;
		}else{
		    enemy.Progress += Time.deltaTime * enemy.ProgressFactor;
            if(enemy.Progress >= 1){
                if(enemy.BlockingTower==null){
                    enemy.Progress = (enemy.Progress - 1f) /enemy.ProgressFactor;
                    if(enemy.PrepareNextState()){
                        enemy.Progress = 0;
                    }else{
                        return;
                    }
                }else{
                    //被阻挡，阻挡的是下一格是移动到下一格，是当前格时不动
                    enemy.Progress = Mathf.Min(1, enemy.Progress);
                    if(enemy.BlockingTower==enemy.TileFrom.Content){
                        return;
                    }
                }
            }
            if (enemy.DirectionChange == DirectionChange.None) {
                enemy.transform.localPosition =
                    Vector3.LerpUnclamped(enemy.positionFrom, enemy.positionTo, enemy.Progress);
            }
            else {
                float angle = Mathf.LerpUnclamped(
                    enemy.directionAngleFrom, enemy.directionAngleTo, enemy.Progress
                );
                enemy.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
            }
        }
    }
    public override void ExecuteTower(GameActor actor, float deltaTime){

    }

    public override void EnterEnemy(GameActor actor)
    {
        Enemy enemy = (Enemy) actor;
		enemy.Animator.PlayMove(enemy.AnimationConfig.MoveAnimationSpeed * enemy.Speed / enemy.Scale);
    }

    public override ActorStateType GetActorType()
    {
        return ActorStateType.Move;
    }
}
