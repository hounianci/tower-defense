using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingTrackerFactory : TrackerFactory
{
    public AbstractTrack CreateTracker(){
        return new BlockingTracker();
    }
}
