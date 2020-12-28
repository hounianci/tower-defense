using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractTrack
{
    //1-敌人 2-队友 3-全部
    int targetType;
    protected GameActor owner;
    public int TargetType{
        get=>targetType;
        set{targetType=value;}
    }
    public GameActor Owner{
        get=>owner;
        set{owner=value;}
    }
    protected Direction direction;
    public Direction Direction{
        get=>direction;
        set{direction=value;}
    }
    
    public void Init(int rangeId, GameActor owner){
        direction = Direction.North;
        this.owner = owner;
        Init0(rangeId);
    }
    public List<GameTile> InRangeGameTile(GameBoard board){
        List<GameTile> inRangeTile = board.targetTailes(new Vector2Int(owner.Tile.X, owner.Tile.Y), RangeSelfOffet(), Range());
        return inRangeTile;
    }
    public void TurnRange(Direction newDirection){
        TurnRange0(newDirection);
        direction = newDirection;
    }

    public virtual List<TargetAble> TrackTarget(int teamId, int trackNum){
        List<TargetAble> result = new List<TargetAble>();
        List<GameTile> inRangeTile = owner.Board.targetTailes(new Vector2Int(owner.Tile.X, owner.Tile.Y), RangeSelfOffet(), Range());
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

    protected virtual void TurnRange0(Direction newDirection){}
    protected virtual Vector2Int RangeSelfOffet(){
        return new Vector2Int(0, 0);
    }
    protected virtual void Init0(int rangeId){}
    public abstract List<List<int>> Range();
}
