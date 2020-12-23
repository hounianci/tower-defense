using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractTrack
{
    //1-敌人 2-队友 3-全部
    int targetType;
    public int TargetType{
        get=>targetType;
        set{targetType=value;}
    }
    Direction direction;
    public Direction Direction{
        get=>direction;
        set{direction=value;}
    }
    
    public void Init(int rangeId){
        Init0(rangeId);
    }
    protected abstract void Init0(int rangeId);
    public abstract List<List<int>> Range();
    public List<GameTile> InRangeGameTile(GameActor tracker, GameBoard board){
        List<GameTile> inRangeTile = board.targetTailes(new Vector2Int(tracker.Tile.X, tracker.Tile.Y), RangeSelfOffet(), Range());
        return inRangeTile;
    }
    public void TurnRange(Direction newDirection){
        TurnRange0(newDirection);
        direction = newDirection;
    }
    protected abstract void TurnRange0(Direction newDirection);
    protected abstract Vector2Int RangeSelfOffet();

    public List<TargetAble> TrackTarget(GameActor tracker, int teamId, int trackNum){
        List<TargetAble> result = new List<TargetAble>();
        List<GameTile> inRangeTile = tracker.Board.targetTailes(new Vector2Int(tracker.Tile.X, tracker.Tile.Y), RangeSelfOffet(), Range());
        bool isFinish = false;
        foreach(GameTile gameTile in inRangeTile){
            foreach(List<TargetAble> v in gameTile.Content.OnboardTargets.Values){
                foreach(TargetAble target in v){
                    int sameTeam = (target.TeamId()^teamId)+1;
                    if((sameTeam&TargetType)!=0){
                        result.Add(target);
                        if(result.Count == trackNum){
                            isFinish = true;
                            break;
                        }
                    }
                }
                if(isFinish){
                    break;
                }
            }
            if(isFinish){
                break;
            }
        }
        return result;
    }

}
