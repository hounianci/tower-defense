using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingTracker : AbstractTrack
{
    public override List<List<int>> Range()
    {
        List<List<int>> range = new List<List<int>>();
        List<int> row1 = new List<int>();
        row1.Add(1);
        row1.Add(1);
        row1.Add(1);
        range.Add(row1);
        List<int> row2 = new List<int>();
        row2.Add(1);
        row2.Add(3);
        row2.Add(1);
        range.Add(row2);
        List<int> row3 = new List<int>();
        row3.Add(1);
        row3.Add(1);
        row3.Add(1);
        range.Add(row3);
        return range;
    }

    protected override bool ValidateTarget(TargetAble target)
    {
        if(!(target is GameActor)){
            return false;
        }
        if(!(owner is BlockerActor)){
            return false;
        }
        GameActor actor = (GameActor) target;
        BlockerActor blocker = (BlockerActor) owner;
        bool isBlock = blocker.IsBlockByMe(actor);
        return isBlock;
    }

}
