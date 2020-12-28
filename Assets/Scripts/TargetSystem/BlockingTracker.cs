using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingTracker : AbstractTrack
{
    public override List<List<int>> Range()
    {
        List<List<int>> range = new List<List<int>>();
        List<int> row = new List<int>();
        row.Add(3);
        range.Add(row);
        return range;
    }
}
